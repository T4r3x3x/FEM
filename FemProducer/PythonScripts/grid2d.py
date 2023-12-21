# -*- coding: cp1251 -*-
from turtle import color
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon

fig = plt.figure()
ax = fig.add_subplot(1, 1, 1, projection='3d')
fig.set_size_inches(12,8)
# Define the vertices for the first polygon
f = open('C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid.txt', 'r', encoding="utf-8")


colors = ["orange","red","green"]

arrayV = []
vertices = []
polygons = []
subdomains = []
temp = []
boundaries = f.readline().split(' ')

ax.set_xlim(float(boundaries[0]), float(boundaries[1]))
ax.set_ylim(float(boundaries[2]), float(boundaries[3]))
#subdomainsCount = int(f.readline())

# for i in range(0,subdomainsCount):
# 	point = f.readline().split(' ')
# 	temp.append(float(point[0]))
# 	temp.append(float(point[1]))		 
# 	temp.append(float(point[2]))
# 	temp.append(float(point[3]))
# 	subdomains.append(temp)

x = [0,1,2]
y= [0,1,2]
z = [0,1,2]
ax.plot_surface(x, y, z, color="g", alpha=0.3)
# elemCount = int(f.readline())

# for line in range(0,elemCount):
# 	for number in range(0,4):
# 		point = f.readline().split(' ')
# 		x = float(point[0])
# 		y = float(point[1])
# 		temp =[]
# 		temp.append(x)
# 		temp.append(y)
# 		vertices.append(temp)
# 	f.readline()
# 	arrayV.append(vertices)
# 	polygons.append(Polygon(arrayV[line], alpha=0.5,  facecolor='none', edgecolor='blue'))
# 	ax.add_patch(polygons[line])
# 	vertices=[]

# for i in range(0,subdomainsCount):
# 	plt.vlines(x=[float(subdomains[i][0]),float(subdomains[i][1])],ymin=float(subdomains[i][2]),ymax=float(subdomains[i][3]),color=colors[i],linewidth = 2)#обводка границ
# 	plt.hlines(y=[float(subdomains[i][2]),float(subdomains[i][3])],xmin=float(subdomains[i][0]),xmax=float(subdomains[i][1]),color=colors[i],linewidth = 2)#скважины
#vertices1 = f.readline()


#Set plot limits and labels


ax.set_xlabel('X-axis')
ax.set_ylabel('Y-axis')
plt.show()