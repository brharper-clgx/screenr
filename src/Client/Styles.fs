module Client.Styles

open Zanaptak.TypedCssClasses

// Font-Awesome classes
type FA = CssClasses<"https://use.fontawesome.com/releases/v5.8.1/css/all.css", Naming.PascalCase>

// Bulma classes
type Bulma = CssClasses<"./node_modules/bulma/css/bulma.css", Naming.PascalCase>
//type Bulma = CssClasses<"https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.css", Naming.PascalCase>

// Custom Styles
//type Style = CssClasses<"./public/styles.scss", Naming.PascalCase, commandFile="sass,Windows=sass.cmd">