﻿/*
* jQuery combinations - v1.0 - 29/10/2011
* http://github.com/bigab/jquery/jquery-combinations-plugin/
*
* Copyright (c) 2011 "BigAB" Adam Barrett
* licensed under the MIT license.
* http://bigab.mit-license.org
* 	@author BigAB - Adam Barrett
*/
(function (a) { a.combinations = function (a) { if (Object.prototype.toString.call(a) !== "[object Array]") { throw new Error("combinations method was passed a non-array argument") } var b = [], c = [], d = a.length ? 1 : 0, e = a.length; for (var f = 0; f < e; ++f) { if (Object.prototype.toString.call(a[f]) !== "[object Array]") { throw new Error("combinations method was passed a non-array argument") } d = d * a[f].length } for (var g = 0; g < d; ++g) { var h = g, c = [], i = []; for (var j = 0; j < e; ++j) { c[j] = h % a[j].length; h = Math.floor(h / a[j].length) } for (var j = 0; j < c.length; ++j) { i.push(a[j][c[j]]) } b.push(i) } return b } })(jQuery)