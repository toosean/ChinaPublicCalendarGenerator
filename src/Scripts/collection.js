//js for http://tools.2345.com/jieri.htm

//传统节日
var chuang_tong = [];
//国际节日
var guo_ji = [];
//节气
var jie_qi = [];

var elements = document.querySelectorAll('.jieri li');

for (var i = 0; i < elements.length; i++) {

    var li = elements[i];
    var a = li.querySelector('a');
    var className = a.className;

    var text = li.innerText;

    var p1 = /(.*?)\[(\d+)\/(\d+)/i;
    var match = text.match(p1);

    var title = match[1];
    var mon = match[2];
    var day = match[3];

    var event = {
      Title: title,
      Begin: "2020-" + mon + "-" + day,
      IsWholeDay: true
    };

    switch(className){
        case "cRed":
            chuang_tong.push(event);
            break;
        case "cGreen":
            guo_ji.push(event);
            break;
        case "cBlue":
            jie_qi.push(event);
            break;
    }
}

var output = {
    chuang_tong,guo_ji,jie_qi
};

//output
JSON.stringify(output,null,4);