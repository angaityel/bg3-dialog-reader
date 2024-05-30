# BG3 Dialog Reader
View, extract, convert, listen Baldur's Gate 3 dialogs.

Based on [LSLib](https://github.com/Norbyte/lslib). Html formatting: TORcommunity.
#
![window](https://github.com/angaityel/bg3-dialog-reader/assets/161797572/5975d5a5-98b6-43cf-af5a-e0d014017451)

For more detailed info and parsed dialogs check: https://www.tumblr.com/roksik-dnd/727481314781102080/bg3-datamined-dialogue-google-drive

How to use:
- Open any *.pak file from Data folder.
- Select language (if multiple installed).
- "Create DB" will create sqlite database (bg3.db) with tags, flags, translation, etc.

Then you can do:
- Extract all dialogs (or one by one) to html.
- Convert all dialogs (or one by one) to Divinity Engine 2 format.
- Listen and export audio (requires [vgmstream](https://github.com/vgmstream/vgmstream/releases)).

## Convert to html
- Click "Export all dialogs to html".

or
- Click "Load dialogs tree" and extract only required dialog.

You can mouseover on some html elements to see additional info:

![html](https://github.com/angaityel/bg3-dialog-reader/assets/161797572/c62db8f2-7d78-4c3d-a968-4e2743011cb2)

## Convert to Divinity Engine 2 format
- Click "Convert".

or
- Click "Load dialogs tree" and extract only required dialog.

More info about Divinity Engine 2:

https://docs.larian.game/Setting_up_the_editor

https://docs.larian.game/Dialog_editor

To see speakers names place "_merged.lsf" into:
```
\Divinity Original Sin 2\DefEd\Data\Public\my_project_name_with_some_scary_numbers\RootTemplates\_merged.lsf
```
You can view dialog tree:
![de](https://github.com/angaityel/bg3-dialog-reader/assets/161797572/293deff9-1e71-4b82-99be-048886a7b96f)

And you can simulate conversation (and you can setup required flags for it):
![ds](https://github.com/angaityel/bg3-dialog-reader/assets/161797572/827bf050-77c9-4d2e-913c-a4ab4dec7f5d)

## Audio
- Click "Load dialogs tree".
- Double-click on the dialog file to load its lines.
- Double-click a line to play it back.
