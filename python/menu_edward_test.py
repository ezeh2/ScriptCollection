#!/usr/bin/python3



# Import the necessary packages
from cursesmenu import *
from cursesmenu.items import *


from cursesmenu import SelectionMenu

a_list = ["red", "blue", "green"]

selection = SelectionMenu.get_selection(a_list)

print(selection)


