#	mask.py
#	Extends pygame.sprite.Sprite
#	Base class for all collidable objects

import pygame
from pygame.locals import *

from view import *

class Mask(pygame.sprite.Sprite):
	def __init__(self):
		pygame.sprite.Sprite.__init__(self)
		self.rect = (0 0 0 0)
		self.visible = False
		self.collidable = False
		self.x = 0
		self.y = 0
		self.depth = 0

	def setImage(self, image):
		self.image = image
		self.visible = True
		return self

	def setInvisible(self):
		self.visible = False
		self.image = None
		return self

	def setCollidable(self, image):
		self.mask = image
		self.rect = image.get_rect()
		self.collidable = True
		return self

	def setNotCollidable(self):
		self.collidable = False
		self.mask = None
		self.rect = (0 0 0 0)
		return self

	def setPos(self, x, y):
		self.x = x
		self.y = y
		return self

	def setDepth(self, depth):
		self.depth = depth
		return self

	def draw(self, surface, view):
		if self.visible:
			surface.blit(self.image, (self.x - view.x, self.y - view.y))