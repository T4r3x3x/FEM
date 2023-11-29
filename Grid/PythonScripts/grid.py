import numpy as np
import matplotlib.pyplot as plt
from matplotlib import cm
import numpy as np

f = open('output\grid.txt', 'r', encoding="utf-8")
boreholes_count = int(f.readline())
boreholes =[]

for line in range(0,boreholes_count):
    boreholes.append(f.readline().split(' '))

array = f.readline().split(' ')
	
x =[]
y =[]
xCount = int(f.readline())
yCount = int(f.readline())
fig = plt.figure(figsize=(16,12))

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

for i in range(0,boreholes_count):
    plt.vlines(x=[float(boreholes[i][0]),float(boreholes[i][1])],ymin=float(boreholes[i][2]),ymax=float(boreholes[i][3]),color='orange',linewidth = 4)#обводка границ
    plt.hlines(y=[float(boreholes[i][2]),float(boreholes[i][3])],xmin=float(boreholes[i][0]),xmax=float(boreholes[i][1]),color='orange',linewidth = 4)#скважины

areasCount = int(f.readline())

for line in range(0,areasCount): #обводка границ подобластей
	array = f.readline().split(' ')
	plt.vlines(x=[float(array[0]),float(array[1])],ymin=float(array[2]),ymax=float(array[3]),color='r',linewidth = 4)
	plt.hlines(y=[float(array[2]),float(array[3])],xmin=float(array[0]),xmax=float(array[1]),color='r',linewidth = 4)
    
xTicks = [] 
yTicks = []       
for i in range(0,xCount): #через один пишем значения х
    if i%2:
        xTicks.append(round(x[i]))
        
for i in range(0,yCount): #через один пишем значения х
    if i%2:
        yTicks.append(round(y[i]))
#plt.xticks(xTicks)
#plt.yticks(yTicks)
plt.tick_params(axis = 'both', which = 'major', labelsize = 16) 
plt.xlabel('x', fontsize=20)  
plt.ylabel('y ', fontsize=20) 
plt.xticks(rotation=45, ha='right')
plt.yticks(rotation=45, ha='right')
plt.savefig('Images\Grid.png', bbox_inches='tight')
fig.set_size_inches(12,9)
plt.show()