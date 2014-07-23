#nowarn "211"
#I "."
#r "Charon.dll"
#r "Deedle.dll"
#r "FSharp.Data.dll"
#r "Foogle.Charts.dll"
open Foogle
open System.Windows.Forms
open Foogle.SimpleHttp
open System.IO
open Deedle
open Charon

type Charon private () =
  static let extractFrameFeatures labelName (frame:Frame<_, _>) = 

    let makeFeature colTyp colKey = 
      let isNumerical = colTyp = typeof<float> || colTyp = typeof<decimal> 
      if isNumerical then
        Numerical(fun (row:ObjectSeries<string>) -> 
          row.TryGet(colKey) |> OptionalValue.asOption)
      else
        Categorical(fun (row:ObjectSeries<string>) -> 
          row.TryGet(colKey) |> OptionalValue.asOption)

    let input = [ for r in frame.Rows.Values -> r, r ]
    let labels = labelName, Categorical(fun (row:ObjectSeries<string>) -> 
      row.TryGet(labelName) |> OptionalValue.asOption)
    let frameData = frame.GetFrameData()
    let features = 
      [ for colKey, (colTyp, _) in Seq.zip frame.ColumnKeys frameData.Columns do
          if colKey <> labelName then
            yield colKey, makeFeature colTyp colKey  ]

    input, (labels, features)

  static member Tree(frame, labelName) =
    let input, (labels, features) = extractFrameFeatures labelName frame
    basicTree input (labels, features) { DefaultSettings with MinLeaf = 1 }

  static member Forest(frame, labelName) = 
    let input, (labels, features) = extractFrameFeatures labelName frame
    forest input (labels, features) { DefaultSettings with MinLeaf = 1 }

let server = ref None
let tempDir = Path.GetTempFileName()
let pid = System.Diagnostics.Process.GetCurrentProcess().Id
let counter = ref 1

do File.Delete(tempDir)
do Directory.CreateDirectory(tempDir) |> ignore

fsi.AddPrinter(fun (chart:FoogleChart) ->
  match !server with 
  | None -> server := Some (HttpServer.Start("http://localhost:8081/", tempDir))
  | _ -> ()
  let file = sprintf "chart_%d_%d.html" pid counter.Value
  File.WriteAllText(Path.Combine(tempDir, file), Internal.chartHtml chart)  
  System.Diagnostics.Process.Start("http://localhost:8081/" + file) |> ignore
  incr counter
  "(Foogle Chart)" )

fsi.AddPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> 
  "\n" + (printer.Format()))

type Foogle.Chart with
  static member PieChart(frame:Frame<_, _>, column, ?label, ?title, ?pieHole) =
    Foogle.Chart.PieChart
      ( frame.GetColumn<float>(column) |> Series.observations, 
        ?label=label, ?title=title, ?pieHole=pieHole)
  static member GeoChart(frame:Frame<_, _>, column, ?label, ?region, ?mode, ?colorAxis, ?sizeAxis) =
    Foogle.Chart.GeoChart
      ( frame.GetColumn<float>(column) |> Series.observations, 
        ?label=label, ?region=region, ?mode=mode, ?colorAxis=colorAxis, ?sizeAxis=sizeAxis)
    
