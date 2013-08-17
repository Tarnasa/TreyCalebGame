#	tileObj.py
#	

import pygame

class TileObj(object):
	def __init__(self):
		self.solid = False
		self.visible = False
		self.image = None

	def draw(self, surface, x, y):
		if (self.visible):
			surface.blit(self.image, (x, y))

	def setImage(self, image):
		self.visible = True
		self.image = image

	def setInvisible(self):
		self.visible = False
