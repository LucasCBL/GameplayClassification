import numpy as np
import pandas as pd

import tensorflow as tf
import pathlib
import json
import os 
import matplotlib.pyplot as plt
import seaborn as sns
import random

from tensorflow import feature_column
from tensorflow import keras
from tensorflow.keras import layers
from sklearn.model_selection import train_test_split
from sklearn.metrics import confusion_matrix
from tensorflow.keras.layers.experimental import preprocessing

os.environ['AUTOGRAPH_VERBOSITY'] = '1'
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2' #to suppress warning given when wxecuting woth conda

def df_to_dataset(dataframe, shuffle=True, batch_size=32):
  dataframe = dataframe.copy()
  labels = dataframe.pop('type')
  ds = tf.data.Dataset.from_tensor_slices((dict(dataframe), labels))
  if shuffle:
    ds = ds.shuffle(buffer_size=len(dataframe))
  ds = ds.batch(batch_size)
  ds = ds.prefetch(batch_size)
  return ds

def get_normalization_layer(name, dataset):
  # Create a Normalization layer for our feature.
  normalizer = preprocessing.Normalization()

  # Prepare a Dataset that only yields our feature.
  feature_ds = dataset.map(lambda x, y: x[name])

  # Learn the statistics of the data.
  normalizer.adapt(feature_ds)

  return normalizer

def get_category_encoding_layer(name, dataset, dtype, max_tokens=None):
  # Create a StringLookup layer which will turn strings into integer indices
  if dtype == 'string':
    index = preprocessing.StringLookup(max_tokens=max_tokens)
  else:
    index = preprocessing.IntegerLookup(max_values=max_tokens)

  # Prepare a Dataset that only yields our feature
  feature_ds = dataset.map(lambda x, y: x[name])

  # Learn the set of possible values and assign them a fixed integer index.
  index.adapt(feature_ds)

  # Create a Discretization for our integer indices.
  encoder = preprocessing.CategoryEncoding(max_tokens=index.vocab_size())

  # Apply one-hot encoding to our indices. The lambda function captures the
  # layer so we can use them, or include them in the functional model later.
  return lambda feature: encoder(index(feature))


dataset = ''
with open("datos.json") as infile:
    dataframe = pd.DataFrame(json.loads(infile.read()))
types = dataframe.type.unique()



i = 0
for bot_type in types:
    dataframe.loc[dataframe.type == bot_type, 'type'] = i
    i += 1

type_count = dataframe['type'].value_counts()
weights = {}
for type in range(len(types)):
  weight =  type_count.max() / (type_count[type] )
  weights[type] = (weight)


type_column = dataframe.type
dataframe.type = pd.to_numeric(type_column)
print(dataframe.info())

train, test = train_test_split(dataframe, test_size=0.2, stratify=dataframe['type'], random_state = 3)
test, val = train_test_split(test, test_size=0.5,stratify=test['type'], random_state = 3) #, random_state = 3

seed = random.randint(1,9999999)

tf.random.set_seed(seed)
print(len(train), 'train examples')
print(len(val), 'validation examples')
print(len(test), 'test examples')

batch_s = 32

train_ds = df_to_dataset(train, batch_size = batch_s)
test_ds = df_to_dataset(test, batch_size = batch_s)
val_ds = df_to_dataset(val, batch_size = batch_s)

[(train_features, label_batch)] = train_ds.take(1)
print('Every feature:', list(train_features.keys()))
print('A batch of ages:', train_features['level_0_death'])
print('A batch of targets:', label_batch )

feature_columns = []

all_inputs = []
encoded_features = []

# Numeric features.
num_features = [
              # 'level_0_damage_dealt', 
                'level_0_damage_dealt_range', 
              #  'level_0_damage_dealt_close',
              #  'level_0_health_lost', 
              # 'level_0_health_lost_range',  # THIS value is always 0 because there are no ranged enemies in level 0
                'level_0_health_lost_close', 
              #  'level_0_kills', 
                'level_0_ranged_kills', 
                'level_0_close_kills', 
              #  'level_0_ranged_enem_killed', # THIS value is always 0 because there are no ranged enemies in level 0
                'level_0_close_enem_killed', 
              #  'level_0_movements', 'level_0_health_regen', 'level_0_turns',
                'level_0_coins', 'level_0_points', 'level_0_prior_deaths', 
              # 'level_1_damage_dealt',
                'level_1_damage_dealt_range', 'level_1_damage_dealt_close', 
              #  'level_1_health_lost',
                'level_1_health_lost_range', 
              #  'level_1_health_lost_close', # THIS value is always 0 because there are no close combat enemies in level 1
              #  'level_1_kills', 
                'level_1_ranged_kills',
                'level_1_close_kills',
                'level_1_ranged_enem_killed', 
              #  'level_1_close_enem_killed',  # THIS value is always 0 because there are no close combat enemies in level 1
                'level_1_movements',
                'level_1_health_regen', 'level_1_turns', 'level_1_coins', 'level_1_points', 'level_1_prior_deaths',
              #  'level_2_damage_dealt',
                'level_2_damage_dealt_range', 'level_2_damage_dealt_close',
              #  'level_2_health_lost', 
                'level_2_health_lost_range', 'level_2_health_lost_close',
              #  'level_2_kills',
                'level_2_ranged_kills', 'level_2_close_kills', 'level_2_ranged_enem_killed', 'level_2_close_enem_killed',
                'level_2_movements', 'level_2_health_regen', 'level_2_turns', 'level_2_coins', 'level_2_points',
                'level_2_prior_deaths',
                'level_3_damage_dealt',
              #  'level_3_damage_dealt_range',
                'level_3_damage_dealt_close', 
              #  'level_3_health_lost', 
                'level_3_health_lost_range',
                'level_3_health_lost_close', 
              #  'level_3_kills', 
                'level_3_ranged_kills', 'level_3_close_kills',
                'level_3_ranged_enem_killed', 'level_3_close_enem_killed', 'level_3_movements', 'level_3_health_regen',
                'level_3_turns', 'level_3_coins', 'level_3_points', 'level_3_prior_deaths',
              #  'level_4_damage_dealt',
                'level_4_damage_dealt_range', 'level_4_damage_dealt_close',
              #  'level_4_health_lost',
                'level_4_health_lost_range', 'level_4_health_lost_close', 
              #  'level_4_kills',
                'level_4_ranged_kills',
                'level_4_close_kills', 'level_4_ranged_enem_killed', 'level_4_close_enem_killed', 'level_4_movements',
                'level_4_health_regen', 'level_4_turns', 'level_4_coins', 'level_4_points', 'level_4_prior_deaths',
               ]

for header in num_features:
  numeric_col = tf.keras.Input(shape=(1,), name=header)
  normalization_layer = get_normalization_layer(header, train_ds)
  encoded_numeric_col = normalization_layer(numeric_col)
  all_inputs.append(numeric_col)
  encoded_features.append(encoded_numeric_col)

for header in ['level_0_death', 'level_1_death', 'level_2_death', 'level_3_death', 'level_4_death']: 
    # Categorical features encoded as integers.
    categorical_col = tf.keras.Input(shape=(1,), name=header, dtype='int64')
    encoding_layer = get_category_encoding_layer(header, train_ds, dtype='int64',
                                                max_tokens=5)
    encoded_age_col = encoding_layer(categorical_col)
    all_inputs.append(categorical_col)
    encoded_features.append(encoded_age_col)


all_features = tf.keras.layers.concatenate(encoded_features)
x = tf.keras.layers.Dense(32, activation="relu")(all_features)
x = tf.keras.layers.Dropout(0.1)(x)
x = tf.keras.layers.Dense(16, activation="relu")(all_features)
x = tf.keras.layers.Dropout(0.1)(x)
x = tf.keras.layers.Dense(32, activation="relu")(x)
output = tf.keras.layers.Dense(6, activation='softmax')(x)
model = tf.keras.Model(all_inputs, output)
model.compile(optimizer='adam',
              loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=False),
              metrics=[ "accuracy"])
# crear checkpoints durante el entrenamiento

checkpoint_path = "training/cp-{epoch:04d}.ckpt"
checkpoint_dir = os.path.dirname(checkpoint_path)
cp_callback = tf.keras.callbacks.ModelCheckpoint(
    filepath = checkpoint_path, 
    verbose = 1, 
    save_weights_only = True,
    save_freq = "epoch")

# pesos iniciales
model.save_weights(checkpoint_path.format(epoch=0))


print(dataframe.info())

tf.keras.utils.plot_model(model, show_shapes=True, rankdir="LR")
epochs = 60
history = model.fit(train_ds, epochs=epochs, validation_data=val_ds, callbacks=[cp_callback], class_weight = weights)
model.save("saved_model/model")
loss, accuracy = model.evaluate(test_ds)
print("Accuracy", accuracy)
acc = history.history['accuracy']
val_acc = history.history['val_accuracy']

loss = history.history['loss']
val_loss = history.history['val_loss']


latest = tf.train.latest_checkpoint(checkpoint_dir)
model = tf.keras.Model(all_inputs, output)
model.compile(optimizer='adam',
              loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=False),
              metrics=["accuracy"])
model.load_weights(latest)
model.evaluate(train_ds, verbose = 2)
model.evaluate(val_ds, verbose = 2)
model.evaluate(test_ds, verbose = 2)




val_attr, val_labels_tensors = tuple(zip(*val_ds))
test_attr, test_labels_tensors = tuple(zip(*test_ds))
train_attr, train_labels_tensors = tuple(zip(*train_ds))


val_features = val_attr[0]
val_labels = np.array(val_labels_tensors[0])
val_pred =  model.predict(val_features)
val_pred = tf.argmax(val_pred,1)
val_confusion_matrix = np.zeros(shape=(6,6))

for i in range(len(val_attr)):
  val_features = val_attr[i]
  val_labels = np.array(val_labels_tensors[i])
  val_pred =  model.predict(val_features)
  val_pred = tf.argmax(val_pred,1)
  val_confusion_matrix += confusion_matrix(val_labels, val_pred, labels=[0,1,2,3,4,5])


test_features = test_attr[0]
test_labels = np.array(test_labels_tensors[0])
test_pred =  model.predict(test_features)
test_pred = tf.argmax(test_pred,1)
test_confusion_matrix = np.zeros(shape=(6,6))

for i in range(len(test_attr)):
  test_features = test_attr[i]
  test_labels = np.array(test_labels_tensors[i])
  test_pred =  model.predict(test_features)
  test_pred = tf.argmax(test_pred,1)
  test_confusion_matrix += confusion_matrix(test_labels, test_pred, labels=[0,1,2,3,4,5])


train_features = train_attr[0]
train_labels = np.array(train_labels_tensors[0])
train_pred =  model.predict(train_features)
train_pred = tf.argmax(train_pred,1)
train_confusion_matrix = np.zeros(shape=(6,6))

for i in range(len(train_attr)):
  train_features = train_attr[i]
  train_labels = np.array(train_labels_tensors[i])
  train_pred =  model.predict(train_features)
  train_pred = tf.argmax(train_pred,1)
  train_confusion_matrix += confusion_matrix(train_labels, train_pred, labels=[0,1,2,3,4,5])





classes = types

print(val_pred)
print(val_labels)
ax1= plt.subplot()
ax1.plot()
sns.heatmap(val_confusion_matrix, annot=True, fmt='g', ax=ax1)  #annot=True to annotate cells, ftm='g' to disable scientific notation


# labels, title and tick
ax1.set_xlabel('Predicted labels')
ax1.set_ylabel('True labels')
ax1.set_title('Confusion Matrix val')
ax1.xaxis.set_ticklabels(classes)
ax1.yaxis.set_ticklabels(classes)
plt.figure()

ax2= plt.subplot()
ax2.plot()
sns.heatmap(train_confusion_matrix, annot=True, fmt='g', ax=ax2)  #annot=True to annotate cells, ftm='g' to disable scientific notation

# labels, title and ticks
ax2.set_xlabel('Predicted labels')
ax2.set_ylabel('True labels')
ax2.set_title('Confusion Matrix train')
ax2.xaxis.set_ticklabels(classes)
ax2.yaxis.set_ticklabels(classes)

plt.figure()
ax3= plt.subplot()
ax3.plot()
sns.heatmap(test_confusion_matrix, annot=True, fmt='g', ax=ax3)  #annot=True to annotate cells, ftm='g' to disable scientific notation

# labels, title and tick
ax3.set_xlabel('Predicted labels')
ax3.set_ylabel('True labels')
ax3.set_title('Confusion Matrix test')
ax3.xaxis.set_ticklabels(classes)
ax3.yaxis.set_ticklabels(classes)


epochs_range = range(epochs)

plt.figure(figsize=(8, 8))
plt.subplot(1, 2, 1)
plt.plot(epochs_range, acc, label='Training Accuracy')
plt.plot(epochs_range, val_acc, label='Validation Accuracy')
plt.legend(loc='lower right')
plt.title('Training and Validation Accuracy')

plt.subplot(1, 2, 2)
plt.plot(epochs_range, loss, label='Training Loss')
plt.plot(epochs_range, val_loss, label='Validation Loss')
plt.legend(loc='upper right')
plt.title('Training and Validation Loss')


print("seed was " + str(seed))

plt.show()

