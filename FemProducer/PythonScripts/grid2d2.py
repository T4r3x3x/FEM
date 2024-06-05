# -*- coding: cp1251 -*-
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon

fig, ax = plt.subplots()
fig.set_size_inches(12,8)
# Define the vertices for the first polygon
f = open('C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\grid2.txt', 'r', encoding="utf-8")


colors = ["orange","red","green"]

arrayV = []
vertices = []
polygons = []
subdomains = []
temp = []
#boundaries = f.readline().split(' ')

ax.set_xlim(-10, 10)
ax.set_ylim(-10, 10)
# ax.set_xlim(float(boundaries[0]), float(boundaries[1]))
# ax.set_ylim(float(boundaries[2]), float(boundaries[3]))
#subdomainsCount = int(f.readline())

# for i in range(0,subdomainsCount):
# 	point = f.readline().split(' ')
# 	temp.append(float(point[0]))
# 	temp.append(float(point[1]))		
# 	temp.append(float(point[2]))
# 	temp.append(float(point[3]))
# 	subdomains.append(temp)

elemCount = int(f.readline())

for line in range(0,elemCount):
	for number in range(0,4):
		point = f.readline().split(' ')
		x = float(point[0])
		z = float(point[2])
		temp =[]
		temp.append(x)
		temp.append(z)
		vertices.append(temp)
	f.readline()
	arrayV.append(vertices)
	polygons.append(Polygon(arrayV[line], alpha=0.5,  facecolor='none', edgecolor='red'))
	ax.add_patch(polygons[line])
	vertices=[]

# for i in range(0,subdomainsCount):
# 	plt.vlines(x=[float(subdomains[i][0]),float(subdomains[i][1])],ymin=float(subdomains[i][2]),ymax=float(subdomains[i][3]),color=colors[i],linewidth = 2)#������� ������
# 	plt.hlines(y=[float(subdomains[i][2]),float(subdomains[i][3])],xmin=float(subdomains[i][0]),xmax=float(subdomains[i][1]),color=colors[i],linewidth = 2)#��������
#vertices1 = f.readline()


#Set plot limits and labels


ax.set_xlabel('X-axis')
ax.set_ylabel('Z-axis')
plt.show()