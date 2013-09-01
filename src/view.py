#	view.py
#

class View(object):
	def __init__(self, viewWidth, viewHeight, roomWidth, roomHeight):
		self.x = 0
		self.y = 0
		self.w = viewWidth
		self.h = viewHeight
		self.maxX = roomWidth - viewWidth
		self.maxY = roomHeight - viewHeight

	def setPos(self, x, y):
		if x < 0:
			x = 0
		if y < 0:
			y = 0
		if x > self.maxX:
			x = self.maxX
		if y > self.maxY:
			y = self.maxY
		self.x = x
		self.y = y

	def centerOn(self, x, y):
		x -= self.w // 2
		y -= self.h // 2
		self.setPos(x, y)
