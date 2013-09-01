#	tileObj.py
#	Represents a tile that makes up a room,

import pygame

import mask

class TileObj(mask.Mask):
	def __init__(self, x, y):
		mask.Mask.__init__(self)
		self.x = x
		self.y = y

	def draw(self, surface):
		if (self.visible):
			surface.blit(self.image, (self.x, self.y))
		return self

	def setImage(self, image):
		self.visible = True
		self.image = image
		self.w = image.get_width()
		self.h = image.get_height()
		return self

	def setInvisible(self):
		self.visible = False
		return self
