#I "bin"
open System.Windows.Forms
#r "Fsharp.Data.dll"

open System.Net
open System
open System.IO
open FSharp.Data

// Fetch the contents of a web page
let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest 
    req.UserAgent <- "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream)
    callback reader url

let GetStreamUrl pReauest pUrlBuilder pUrlPart =
    let fullUrl:string = pUrlBuilder pUrlPart
    pReauest fullUrl
    

let myCallback (reader:IO.StreamReader) url = 
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

let GetLinkNameUrl pGetStreamUrl pUrlPart name listT = 
    let pUrlSteam:string = pGetStreamUrl pUrlPart
    let logFile = Path.Combine(__SOURCE_DIRECTORY__, "log.txt")
    File.WriteAllText(logFile, pUrlSteam)
    let doc = HtmlDocument.Parse pUrlSteam
    let desc = doc.Descendants "ul" |> Seq.toList
    let css = doc.CssSelect "ul" |> Seq.toList
    printfn "desc: %A" desc.Length
    printfn "css: %A" css.Length
    for ul in css do
        printfn "- ul: %A" ul
    ""

let FullUrl l r = 
    sprintf "%s%s" l r

let baseUrl = "https://www.amazon.com"

let url = 
    GetLinkNameUrl 
            (GetStreamUrl (fetchUrl myCallback) (FullUrl baseUrl))  
            "/s/ref=lp_1_nr_n_0?fst=as%3Aoff&rh=n%3A283155%2Cn%3A%211000%2Cn%3A1%2Cn%3A173508&bbn=1&ie=UTF8&qid=1519043803&rnid=1"
            "123" 
            List.empty

let logFile = Path.Combine(__SOURCE_DIRECTORY__, "log.html")
let html = logFile |> File.ReadAllText |> HtmlDocument.Parse
//let c1 = "s-result-list s-col-1 s-col-ws-1 s-result-list-hgrid s-height-equalized s-list-view s-text-condensed"
//let c1 = "s-result-list"
//html.CssSelect "ul" |> Seq.filter (fun ul -> (ul.AttributeValue "class").Contains c1)

html.Descendants "div" |> Seq.filter (fun ul -> (ul.AttributeValue "id").Contains "mainResults")
html.CssSelect "#mainResults"

let container = html.CssSelect "#container" |> Seq.head
let containerHtml = container.ToString()
let i = containerHtml.IndexOf "mainResults"
containerHtml.Substring(i-10, 20)

//container.CssSelect "#mainResults"

container.Descendants("div") |> Seq.length


let out = Path.Combine(__SOURCE_DIRECTORY__, "log2.html")
File.WriteAllText(out, html.ToString())

