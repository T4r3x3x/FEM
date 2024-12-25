# -*- coding: cp1251 -*-
import sys
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon

def GetElemsFromPoint(point, section2D):
	if section2D == "xy":
		return float(point[0]), float(point[1])
	if section2D == "xz":
		return float(point[0]), float(point[2])
	if section2D == "yz":
		return float(point[1]), float(point[2])
	else:
		raise Exception("Wrong section!")

args = sys.argv[1:]
sourceFilePath = args[0]
windowTitle = args[1]
#linesColor = args[2]
#section = args[3]

colors = ["orange","red","green"]

fig, ax = plt.subplots(2)
fig.set_size_inches(12,8)
fig.canvas.manager.set_window_title(windowTitle)

f = open(sourceFilePath, 'r', encoding="utf-8")

verticesXY = []
verticesXZ = []
polygonsXY = []
polygonsXZ = []
subdomains = []
temp = []
temp2 = []
#boundaries = f.readline().split(' ')

leftBoundary = -160
rightBoundary = 160
ax[0].set_xlim(leftBoundary, rightBoundary)
ax[0].set_ylim(leftBoundary, rightBoundary)
ax[1].set_xlim(leftBoundary, rightBoundary)
ax[1].set_ylim(leftBoundary, rightBoundary)
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
		x, y = GetElemsFromPoint(point, 'xy')
		temp = []
		temp.append(x)
		temp.append(y)
		verticesXY.append(temp)
		x2, y2 = GetElemsFromPoint(point, 'xz')
		temp2 = []
		temp2.append(x2)
		temp2.append(y2)
		verticesXZ.append(temp2)
	f.readline()
	polygonsXY.append(Polygon(verticesXY, alpha=0.5,  facecolor='none', edgecolor="red"))
	polygonsXZ.append(Polygon(verticesXZ, alpha=0.5,  facecolor='none', edgecolor="blue"))
	ax[0].add_patch(polygonsXY[line])
	ax[1].add_patch(polygonsXZ[line])
	verticesXY=[]
	verticesXZ=[]

# for i in range(0,subdomainsCount):
# 	plt.vlines(x=[float(subdomains[i][0]),float(subdomains[i][1])],ymin=float(subdomains[i][2]),ymax=float(subdomains[i][3]),color=colors[i],linewidth = 2)#������� ������
# 	plt.hlines(y=[float(subdomains[i][2]),float(subdomains[i][3])],xmin=float(subdomains[i][0]),xmax=float(subdomains[i][1]),color=colors[i],linewidth = 2)#��������
#vertices1 = f.readline()

ax[0].set_xlabel('x',fontsize=15)
ax[0].set_ylabel('y',fontsize=15)
ax[1].set_xlabel('x',fontsize=15)
ax[1].set_ylabel('z',fontsize=15)
ax[0].set_aspect('equal', adjustable='box')
ax[1].set_aspect('equal', adjustable='box')
plt.show()