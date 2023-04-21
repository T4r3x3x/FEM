import numpy as np
import matplotlib.pyplot as plt
from matplotlib import cm
import numpy as np

f = open('grid.txt', 'r', encoding="utf-8")
borehole = f.readline().split(' ')

array = f.readline().split(' ')
	
x =[]
y =[]
xCount = int(f.readline())
yCount = int(f.readline())

for line in range(0,xCount):	
	_x = float(f.readline())
	x.append(_x)
	plt.vlines(x=_x,ymin=float(array[2]),ymax=float(array[3]),color="gray")

for line in range(0,yCount):
	_y = float(f.readline())
	y.append(_y)
	plt.hlines(y=_y,xmin=float(array[0]),xmax=float(array[1]),color="gray")

plt.vlines(x=[float(array[0]),float(array[1])],ymin=float(array[2]),ymax=float(array[3]),color='r',linewidth = 4)#обводка границ 
plt.hlines(y=[float(array[2]),float(array[3])],xmin=float(array[0]),xmax=float(array[1]),color='r',linewidth = 4)#расчётной области

plt.vlines(x=[float(borehole[0]),float(borehole[1])],ymin=float(borehole[2]),ymax=float(borehole[3]),color='orange',linewidth = 4)#обводка границ
plt.hlines(y=[float(borehole[2]),float(borehole[3])],xmin=float(borehole[0]),xmax=float(borehole[1]),color='orange',linewidth = 4)#скважины

areasCount = int(f.readline())

for line in range(1,areasCount): #обводка границ подобластей
	array = f.readline().split(' ')
	plt.vlines(x=[float(array[0]),float(array[1])],ymin=float(array[2]),ymax=float(array[3]),color='r',linewidth = 4)
	plt.hlines(y=[float(array[2]),float(array[3])],xmin=float(array[0]),xmax=float(array[1]),color='r',linewidth = 4)
plt.xticks(x)
plt.yticks(y)
plt.show()