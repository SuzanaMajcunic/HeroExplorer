# HeroExplorer

HeroExplorer is a Universal Windows application made as an exercise in training course "Windows 10 Development for Absolute Beginners" at the Microsoft Virtual Academy. Through the app, you can explore the universe of Marvel's characters and their comic books.

This is a single-page app, divided into three visual sections. Left column represents a list of randomly chosen Marvel characters with corresponding icons and name. After choosing one character, on the right part of the screen appears a section with character description (his full size picture and short description) and corresponding list of comics. Clicking on particular comic, the third section appears and shows the exact name of chosen comic, full size comic cover and a short description.

The code is implemented by applying structural pattern called Facade, a single class that represents an entire subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.

Comics data are fetched by calling Marvel Comics API. The result came in JSON format, so for generating C# classes from JSON I have used online tool 'json2csharp'.

![alt text](https://github.com/SuzanaMajcunic/HeroExplorer/blob/master/print-screen-app-HeroExplorer.PNG?raw=true "Home page HeroExplorer")
