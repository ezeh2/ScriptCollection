
import {Point, Color2} from './point'
// import {} from 'console'


var x =2;

let y: number = 12;

let z = (x) => {
    console.log(x);
}


let point = new Point();

point.x =2;
point.y = 3;

class Auto
{
    constructor(private _color: string, private _height?: number) {
    }

    set Color(color: string) {
        this._color = color;
    }

};

let auto1: Auto = new Auto("yellow", 100);
let auto2: Auto = new Auto("yellow");
auto1.Color = "brown";

let c: Color2 = Color2.green;

console.log(auto2);

