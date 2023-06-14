import imp
from traceback import print_tb
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
import numpy as np
import sys
from matplotlib.colors import ColorConverter
from matplotlib.collections import PatchCollection
from matplotlib.widgets import Slider, Button
import matplotlib
import matplotlib as mpl
from matplotlib import colors
from functools import partial
import heapq
import random
import keyboard

_x=[]
_y=[]
_t=[]
x = []
y = []
t = []

f = open('temperature.txt', "r")
size = int(f.readline())
layersCount = int(f.readline())

for layer in range(0,layersCount):
	for line in range(0,size):        
		arr = list(f.readline().split(" "))
		x.append(float(arr[0]))
		y.append(float(arr[1]))
		t.append(float(arr[2]))
	_x.append(x.copy())
	_y.append(y.copy())
	_t.append(t.copy())
	x.clear()
	y.clear()
	t.clear()

fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.ticklabel_format(useOffset=False)
fig = plt.gcf()
fig.canvas.manager.set_window_title("temperature")
ax.plot_trisurf(_x[0], _y[0],_t[0], linewidth=0, antialiased=False,
               label='pressure', shade=True, cmap=cm.inferno)


def draw(layer):
	ax.cla()
	ax.plot_trisurf(_x[0], _y[0],_t[int(layer)], linewidth=0, antialiased=False,
               label='pressure', shade=True, cmap=cm.inferno)
	

def set_slider(s, val):
    s.val = round(val)
    s.poly.xy[2] = s.val, 1
    s.poly.xy[3] = s.val, 0
    s.valtext.set_text(s.valfmt % s.val)
    draw(s.val-1)


axfreq = plt.axes([0.25, 0.1, 0.65, 0.03])
elem_slider = Slider(
    ax=axfreq,
    label='Time',
    valmin=1,
    valmax=layersCount,
    orientation='horizontal',
    valinit=0.,
    valfmt="%i"
)
elem_slider.on_changed(partial(set_slider, elem_slider))
draw(0)
plt.show()