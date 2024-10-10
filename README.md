# OmiLAXR.Pseudonymization

## Module dependencies
The adapter has following third-party dependencies:

- `PseudonymProvider`: Is using the Web Service https://mtlg.elearn.rwth-aachen.de/pseudo/ and need internet connection. If it is not working the Web Service may be offline.

## Install by using scoped registry
1. Ensure in "Project settings" > "Package Manager" that you have the scoped registry with following settings:
    - Name: npmjs
    - URL: http://registry.npmjs.com
    - Scope(s): `com.rwth.unity.omilaxr.pseudonymization`
2. Go to Package Manager.
3. Click on the (+) button.
4. Select 'Add package by name'.
5. Place in 'Name' field: `com.rwth.unity.omilaxr.pseudonymization`.

### Adding scoped registry by using manifest.json (also recommended - quick way)
1. Alternatively, instead of adding the scoped registry inside Unity editor you can do it by using `manifest.json` file.
2. Go to you project root and then open `Packages/manifest.json`.
3. Ensure following entries in your file: `"scopedRegistries": [
   {
   "name": "npmjs",
   "url": "http://registry.npmjs.com/",
   "scopes": [
   "com.rwth.unity.omilaxr.pseudonymization"
   ]
   }]`.
4. By the way, you can also add here this package by adding `"com.rwth.unity.omilaxr.pseudonymization": "^1.0.0"` to the dependencies (attention you can change the version).


## Install by using Git url
1. Go to Package Manager.
2. Click on the (+) button.
3. Select 'Add package from git URL'.
4. Paste `https://gitlab.com/learntech-rwth/omilaxr-ecosystem/v2/omilaxr.pseudnomization.git` and confirm.

## For Developers

To work with this package we recommend to place it somewhere outside your Unity project (if the package gets an own git repository) or in root of your project.
Than, you can include the package into your project by going to `Window > Package Manager`, click on `(+)` button and finally import the `package.json` of this project by clicking on `Add package from disk`.

For production use we recommend to use `Add package form git URL` or using scoped registries (see below).

## Available Pseudonym Generators

Here you can see the available pseudonym generators. All of them have a prefab available.

- `PseudonymProvider`: Is generating an 100%-safe-unique pseudonym by using the web service https://mtlg.elearn.rwth-aachen.de/pseudo/. 
- `RandomPseudonymGenerator`: Is generating a random Pseudonym with a chartset of "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".
- ... coming soon ... You are invited to contribute ;)