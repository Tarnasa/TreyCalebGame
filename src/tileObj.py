#	tileObj.py
#	

class TileObj(object):
	def __init__(self):
		self.solid = False
		self.visible = False
		self.image = None

	def __init__(self, image):
		self.solid = False
		self.visile = True
		self.image = image
