import pygame
import math

pygame.init()

screen = pygame.display.set_mode((200, 200))
run = True
pos = (100, 100)
clock = pygame.time.Clock()

# speed of your player
speed = 2

# key bindings
move_map = {pygame.K_LEFT: (-1, 0),
            pygame.K_RIGHT: (1, 0),
            pygame.K_UP: (0, -1),
            pygame.K_DOWN: (0, 1)}

while run:
  screen.fill((0, 255, 0))
  # draw player, but convert position to integers first
  pygame.draw.circle(screen, (255, 0, 0), map(int, pos), 10)
  pygame.display.flip()

  # determine movement vector
  pressed = pygame.key.get_pressed()
  move_vector = (0, 0)
  for m in (move_map[key] for key in move_map if pressed[key]):
    move_vector = map(sum, zip(move_vector, m))

  # normalize movement vector if necessary
  if sum(map(abs, move_vector)) == 2:
    move_vector = [p/1.4142 for p in move_vector]

  # apply speed to movement vector
  move_vector = [speed*p for p in move_vector]

  # update position of player
  pos = map(sum, zip(pos, move_vector))

  for e in pygame.event.get():
    if e.type == pygame.QUIT: run = False

  clock.tick(60)