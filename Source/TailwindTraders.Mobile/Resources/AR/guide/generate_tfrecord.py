from __future__ import division
from __future__ import print_function
from __future__ import absolute_import

import os
import io
import os.path
import pandas as pd
import tensorflow as tf
from string import Template

from PIL import Image
from collections import namedtuple, OrderedDict

def int64_feature(value):
  return tf.train.Feature(int64_list=tf.train.Int64List(value=[value]))


def int64_list_feature(value):
  return tf.train.Feature(int64_list=tf.train.Int64List(value=value))


def bytes_feature(value):
  return tf.train.Feature(bytes_list=tf.train.BytesList(value=[value]))


def bytes_list_feature(value):
  return tf.train.Feature(bytes_list=tf.train.BytesList(value=value))


def float_list_feature(value):
  return tf.train.Feature(float_list=tf.train.FloatList(value=value))

def split(df, group):
    data = namedtuple('data', ['filename', 'object'])
    gb = df.groupby(group)
    return [data(filename, gb.get_group(x)) for filename, x in zip(gb.groups.keys(), gb.groups)]


def create_tf_example(group, path, map_dict):
    with tf.gfile.GFile(os.path.join(path, '{}'.format(group.filename)), 'rb') as fid:
        encoded_img = fid.read()
    encoded_img_io = io.BytesIO(encoded_img)
    image = Image.open(encoded_img_io)
    width, height = image.size

    filename = group.filename.encode('utf8')
    image_format = b'jpg'
    xmins = []
    xmaxs = []
    ymins = []
    ymaxs = []
    classes_text = []
    classes_label = []

    for index, row in group.object.iterrows():
        xmins.append(row['xmin'] / width)
        xmaxs.append(row['xmax'] / width)
        ymins.append(row['ymin'] / height)
        ymaxs.append(row['ymax'] / height)
        classes_text.append(row['class_text'].encode('utf8'))
        classes_label.append(map_dict.get(row['class_text'], 0))

    tf_example = tf.train.Example(features=tf.train.Features(feature={
        'image/height': int64_feature(height),
        'image/width': int64_feature(width),
        'image/filename': bytes_feature(filename),
        'image/source_id': bytes_feature(filename),
        'image/encoded': bytes_feature(encoded_img),
        'image/format': bytes_feature(image_format),
        'image/object/bbox/xmin': float_list_feature(xmins),
        'image/object/bbox/xmax': float_list_feature(xmaxs),
        'image/object/bbox/ymin': float_list_feature(ymins),
        'image/object/bbox/ymax': float_list_feature(ymaxs),
        'image/object/class/text': bytes_list_feature(classes_text),
        'image/object/class/label': int64_list_feature(classes_label),
    }))
    return tf_example


def main(_):

    map_file = 'csv/label_map.csv'
    if not (os.path.isfile(map_file)):
        print ('{} was not found!'.format(map_file))
        exit()
    
    map = pd.read_csv(map_file)
    map_dict = dict(map.values)
    
    if not os.path.exists('records'):
        os.makedirs('records')
    
    f_map = open('records/label_map.pbtxt','w')

    for key, value in map_dict.items():
        data = """item {
    id: $val
    name: '$key'
    display_name: '$key'
}
"""
        
        line = Template(data).substitute(val=value, key=key)
        f_map.write(line)
    
    f_map.close()

    for directory in ['train','test']:
        output_path = os.path.join(os.getcwd(), 'records/{}.record'.format(directory))
        writer = tf.python_io.TFRecordWriter(output_path)
        path = os.path.join(os.getcwd(), 'images', directory)

        examples = pd.read_csv('csv/{}.csv'.format(directory))
        grouped = split(examples, 'filename')
        for group in grouped:
            tf_example = create_tf_example(group, path, map_dict)
            writer.write(tf_example.SerializeToString())

        writer.close()

        print('Successfully created the TFRecords: {}'.format(output_path))


if __name__ == '__main__':
    tf.app.run()