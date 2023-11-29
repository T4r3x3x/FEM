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
_x=[]
_y=[]
_t=[]
x = []
y = []
t = []

f = open(r'output\temperature.txt', 'r', encoding="utf-8")
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


fig = plt.figure(figsize=(27,18))
ax = plt.axes(projection='3d')
fig = plt.gcf()
fig.canvas.manager.set_window_title("temperature")
ax.plot_trisurf(_x[0], _y[0],_t[0], linewidth=0, antialiased=False,
               label='temperature', shade=True, cmap=cm.inferno)
plt.xlabel('x', fontsize=30, labelpad =30)  
plt.ylabel('y ', fontsize=30, labelpad =30) 

def draw(layer):
	ax.cla()
	ax.plot_trisurf(_x[0], _y[0],_t[int(layer)], linewidth=0, antialiased=False,label='temperature', shade=True, cmap=cm.inferno)
	ax.set_ylabel('y ', fontsize=30, labelpad =30)
	ax.set_xlabel('x ', fontsize=30, labelpad =30)
	ax.tick_params(axis = 'both', which = 'major', labelsize = 24)


	

def set_slider(s, val):
    s.val = round(val)
    s.poly.xy[2] = s.val, 1
    s.poly.xy[3] = s.val, 0
    s.valtext.set_text(s.valfmt % s.val)
    draw(s.val-1)


axfreq = plt.axes([0.20, 0.00, 0.65, 0.03])
elem_slider = Slider(
    ax=axfreq,
    label='Time',
    valmin=1,
    valmax=layersCount,
    orientation='horizontal',
    valinit=layersCount,
    valfmt="%i"
)
elem_slider.label.set_size(32)
elem_slider.valtext.set_size(32)

elem_slider.on_changed(partial(set_slider, elem_slider))
fig.set_tight_layout(True)
plt.tick_params(axis = 'both', which = 'major', labelsize = 34) 
draw(layersCount-1)
plt.savefig('Images\Temperature.png', bbox_inches='tight') 
fig.set_size_inches(12,9)
plt.show()