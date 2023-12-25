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
X = []
y = []
T = []


f = open("C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\solve.txt", 'r', encoding="utf-8")
size = int(f.readline())
layersCount = int(f.readline())

for layer in range(0,layersCount):
	for line in range(0,size):        
		arr = list(f.readline().split(" "))
		X.append(float(arr[0]))
		y.append(float(arr[1]))
		T.append(float(arr[2]))
	_x.append(X.copy())
	_y.append(y.copy())
	_t.append(T.copy())
	X.clear()
	y.clear()
	T.clear()


fig, ax = plt.subplots()
# fig = plt.gcf()
fig.canvas.manager.set_window_title("temperature")
tricount = ax.tricontourf(_x[0], _y[0],_t[0], linewidth=0, antialiased=False,
               label='temperature', shade=True, cmap=cm.inferno)
colorbar = fig.colorbar(tricount, ax=ax,location='right')  
plt.xlabel('X', fontsize=30, labelpad =30)  
plt.ylabel('y ', fontsize=30, labelpad =30) 

def draw(layer):
	ax.cla()
	# ax.plot_trisurf(_x[0], _y[0],_t[int(layer)], linewidth=0, antialiased=False,label='temperature', shade=True, cmap=cm.inferno)
	tricount = ax.tricontourf(_x[0], _y[0],_t[int(layer)], linewidth=0, antialiased=False,label='temperature', shade=True, cmap=cm.inferno)
	ax.set_ylabel('y ', fontsize=30, labelpad =30)
	ax.set_xlabel('X ', fontsize=30, labelpad =30)
	ax.tick_params(axis = 'both', which = 'major', labelsize = 24)
	global colorbar
	colorbar.remove()
	colorbar = fig.colorbar(tricount, ax=ax,location='right')  


	

def set_slider(s, val):
    s.val = round(val)
    s.poly.xy[2] = s.val, 1
    s.poly.xy[3] = s.val, 0
    s.valtext.set_text(s.valfmt % s.val)
    draw(s.val-1)


axfreq = plt.axes([0.20, 0.00, 0.65, 0.03])
elem_slider = Slider(
    ax=axfreq,
    label='Z',
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
# plt.savefig('Images\Temperature.png', bbox_inches='tight') 
fig.set_size_inches(12,9)
plt.show()