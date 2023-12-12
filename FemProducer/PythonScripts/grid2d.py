import matplotlib.pyplot as plt
from matplotlib.patches import Polygon

fig, ax = plt.subplots()

# Define the vertices for the first polygon
f = open('grid2.txt', 'r', encoding="utf-8")
# boundaries = f.readline().split(' ')
# ax.set_xlim(float(boundaries[0]), float(boundaries[1]))
# ax.set_ylim(float(boundaries[2]), float(boundaries[3]))
elemCount = int(f.readline())
arrayV = []
vertices = []
polygons = []

for line in range(0,elemCount):
	for number in range(0,4):
		point = f.readline().split(' ')
		x = float(point[0])
		y = float(point[1])
		temp =[]
		temp.append(x)
		temp.append(y)
		vertices.append(temp)
	f.readline()
	arrayV.append(vertices)
	polygons.append(Polygon(arrayV[line], alpha=0.5,  facecolor='none', edgecolor='blue'))
	ax.add_patch(polygons[line])
	vertices=[]


#vertices1 = f.readline()


#Set plot limits and labels


ax.set_xlabel('X-axis')
ax.set_ylabel('Y-axis')
plt.show()