from turtle import xcor
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.tri as tri 
from matplotlib.patches import Polygon

# def getMaskz(xmid, ymid, unz):    
#     mask = []
#     for i in range(len(xmid)):
#         flag = True
#         p = [xmid[i], ymid[i], unz]   
#         for j in range(len(subdomains)):
#             if pointInside(np.array(subdomains[j]), np.array(p)):       
#                 flag = False
#                 break
#         mask.append(flag)
#     return mask

# fig, ax = plt.subplots()

# # Define the vertices for the first polygon
# f = open('isolines.txt', 'r', encoding="utf-8")
# #elemCount = int(f.readline())
# x = []
# y = []
# z = []
# mask = []
# temp = []
# subdomains = []
# subdomainsCount = int(f.readline())

# for i in range(0,subdomainsCount):
# 	point = f.readline().split(' ')
# 	temp.append(float(point[0]))
# 	temp.append(float(point[1]))		
# 	temp.append(float(point[2]))
# 	temp.append(float(point[3]))
# 	subdomains.append(temp)
# 	temp =[]


# xCount = int(f.readline())
# yCount = int(f.readline())

# for line in range(0,yCount):
# 	for i in range(0,xCount):
# 		point = f.readline().split(' ')
# 		x.append(float(point[0]))
# 		y.append(float(point[1]))		
# 		z.append(float(point[2]))
	

# # Z_masked = np.ma.masked_array(z, mask)

# triang = tri.Triangulation(x, y) 
# ax.set_xlabel('X-axis')
# ax.set_ylabel('Y-axis')
# xmid = x[triang.triangles].mean(axis=1)
# ymid = y[triang.triangles].mean(axis=1)

# triang.set_mask(mask)
# # _x = sorted(list(set(x)))
# # _y = sorted(list(set(x)))
# plt.tricontourf(triang, z)
# plt.colorbar()
# plt.show()

# You can specify your own triangulation rather than perform a Delaunay
# triangulation of the points, where each triangle is given by the indices of
# the three points that make up the triangle, ordered in either a clockwise or
# anticlockwise manner.

