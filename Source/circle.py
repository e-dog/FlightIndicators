import Image, ImageDraw


s=4

im=Image.new('RGBA', (256*s, 211*s), (0, 0, 0, 0))
draw=ImageDraw.Draw(im)
x=121.10
y=104.54
r=120.23-6
draw.ellipse((x*s-r*s, y*s-r*s, x*s+r*s, y*s+r*s), fill=(255, 255, 255, 255))
im=im.resize((256, 211), Image.ANTIALIAS)
im.show()
