# MSW Map Converter

The MSW Map Converter is a tool designed to convert maps from the MapleStory client files, which were initially extracted using [WzDumper](https://github.com/Xterminatorz/WZ-Dumper), into a format compatible with MapleStory Worlds. It achieves this by saving the maps into an already extracted `.mod` folder using [ShortSwordSlicer](https://github.com/SeokguKim/ShortSwordSlicer).

## Overview

This project provides a GUI interface that allows users to select multiple maps from a specific region and convert them into MapleStory Worlds format. The tool focuses on converting key attributes such as footholds, ropes, ladders, and tiles from the original maps. 

**Important:** This project is in a very early stage of development. Currently, only basic elements like footholds, ropes, ladders, and tiles are processed and converted, and are not yet fine-tuned. More features and map elements will be supported in future releases.

## Features

- **Map Extraction from MapleStory Client Files**: Uses WzDumper to extract .wz files into a readable format.
- **Efficient Map Conversion**: Converts maps into a format suitable for MapleStory Worlds.
- **User-friendly GUI**: Provides a graphical interface to select and convert multiple maps at once from a chosen region.
- **Early Support Elements**: Initially supporting footholds, ropes, ladders, and tiles.

## Requirements

- **WzDumper**: Essential for extracting MapleStory client files before conversion.
- **ShortSwordSlicer**: Required for processing extracted `.mod` folder contents.

## Usage

1. **Extract MapleStory Files**: Use WzDumper to extract the MapleStory client files.
2. **Prepare .mod Folder**: Ensure the `.mod` folder is properly set up with ShortSwordSlicer.
3. **Run the Converter**:
   - Start the application.
   - Select your desired MapleStory region.
   - Choose multiple maps to convert.
   - Begin the conversion process.

Upon completion, your selected maps, now in MapleStory Worlds format, will be stored in the specified `.mod` folder.

## Limitations

- **Basic Conversion**: The current version only converts fundamental map elements. It does not yet support complex interactions or dynamic elements.
- **Early Development Stage**: As this project is still in development, expect enhancements and improvements in upcoming updates.

## Future Work

- Add support for additional map elements and interactions.
- Expand the user interface with enhanced options and controls.
- Improve conversion accuracy and performance.

## License

This project is open-source and licensed under the terms of the MIT license.

## Contributions

Contributions are welcome! Feel free to submit issues or pull requests to help the project grow.

---

We hope this tool aids you in exploring the vast worlds of MapleStory Worlds in new and exciting ways. Stay tuned for updates and new features!