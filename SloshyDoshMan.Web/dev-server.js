const express = require('express')
const request = require('request')
const app = express()
const port = 3000

app.use(express.static("dist"))
app.use("/", function(req, res) {
    var url = 'http://192.168.1.25:8098' + req.url
    var r = null

    if (req.method === 'POST') {
       r = request.post({uri: url, json: req.body})
    } else {
       r = request(url)
    }
  
    req.pipe(r).pipe(res)
});

app.listen(port, () => console.log(`Dev Server started on port ${port}!`))