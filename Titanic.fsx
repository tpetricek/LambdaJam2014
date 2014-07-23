#load "lib/FsLab.fsx"
open FsLab
open Foogle
open Charon
open Deedle
open FSharp.Data

// ------------------------------------------------------------------
// Type providers
// ------------------------------------------------------------------

// Load the titanic data set using the CSV type provider and print 
// the names of all the surviving passangers who were older than 60
//
// To do that, you can use CsvProvider<...> where ... is the name
// of the file that you want to load (here "titanic.csv"). Then 
// name the created type using "type T = CsvProvider<...>" and load
// the sample data using "let data = T.GetSample()". Finally,
// get the list of rows using "data.Rows" and iterate over it using
// an imperative "for" loop.


// (...)


// ------------------------------------------------------------------
// Deedle series
// ------------------------------------------------------------------

// Next, load the data into a Deedle series. A series is a mapping 
// from keys to values. A single pair is created using "k => v".
// To create a series, you need to use the "series [ .. ]" function.
// You could for instance create a series like this:
// 
//     let langs = series [ "Ada" => 1980; "Basic" => 1964; "Clojure" => 2007 ]
//
// As another example, the following creates a series of squares:
// 
//     let squares = series [ for i in 1 .. 10 -> i => i * i]
//
// First, we want to create a series mapping passenger names to a
// Boolean value specifying if they survived or not.
//
// Next, calculate the ratio of how many people survived. To do
// that, transform the series using the "Series.mapValues" function
// and then use "Stats.mean". The following example counts how many
// of the squares are odd:
// 
//     squares 
//     |> Series.mapValues (fun v -> if v%2 = 0 then 1.0 else 0.0) 
//     |> Stats.sum


// (...)


// ------------------------------------------------------------------
// Deedle frames and Charon
// ------------------------------------------------------------------

// Next, we need to create a data frame containing multiple features
// from the data set. To do that, create a data frame that contains
// the single series we have (survived) and then add additional series
// The following example combines squares and cubes:
//
//     let nums = frame [ "Squares" => squares ]
//     nums?Cubes <- series [ for i in 1 .. 10 -> i => i*i*i ]
//
// Write code to add 'Sex', 'Pclass' and other features. You can run
// individual lines in F# interactive to try different features.


// (...)


// Now, we use "Charon" library which implements decision trees and 
// random forests for F#. To create a tree, call "Charon.Tree" and 
// give it a data frame with features and a column representing the 
// expected result. Here is a basic example:

let super = 
  series [ "Tomas" => false; "Mathias" => false; "Rick" => false; 
           "Batman" => true; "Superman" => true; "Spiderman" => true; ]

let heroes = frame [ "Superhero" => super ]
heroes?Costume <- [ false; false; false; true; true; true; ]
heroes?BornInAmerica <- [ false; false; true; true; false; true; ]

let heroTree = Charon.Tree(heroes, "Superhero")
printfn "%s" heroTree.Pretty

// Create a tree from the Titanic features. What combination of features
// gives us the best tree? What features are not useful to add?


// (...)


