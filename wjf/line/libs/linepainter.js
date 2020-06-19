/*
html5行情图库
author:yukaizhao
blog:http://www.cnblogs.com/yukaizhao/
商业或公开发布请联系：yukaizhao@gmail.com
*/
/*
options = {
  region:{x:10,y:10,width:300,height:200},
  maxDotsCount:241,
  getDataLength:function(){},
  getItemValue:function(item){return item.price;},
  middleValue: 10.4, //通常是昨收
  color:'blue'
}
*/
function linePainter(options){
  this.options = options;
}

linePainter.prototype = {
    initialize:function(absPainter){
      absPainter.options  = this.options;
    },
    getDataLength:function(){return this.options.getDataLength.call(this);},
    getX: function (i) {
        return (i + 1) * (this.options.region.width / this.options.maxDotsCount);
    },
    start: function () {
        var ctx = this.ctx;
        var options = this.options;
        var region = options.region;
        ctx.save();
        //转换坐标
        ctx.translate(region.x, region.y + region.height / 2);

        var maxDiff = 0;
        var me = this;
        
        this.data.items.each(function (item) {
            var diff = Math.abs(options.middleValue - options.getItemValue(item));
            maxDiff = Math.max(diff, maxDiff);
        });

        this.maxDiff = maxDiff;
        ctx.beginPath();
        ctx.strokeStyle = options.lineColor;
    },
    startArea: function () {
        var ctx = this.ctx;
        var options = this.options;
        var region = options.region;
        ctx.save();
        //转换坐标
        ctx.translate(region.x, region.y + region.height / 2);
        var maxDiff = 0;
        var me = this;
        
        this.data.items.each(function (item) {
            var diff = Math.abs(options.middleValue - options.getItemValue(item));
            maxDiff = Math.max(diff, maxDiff);
        });

        this.maxDiff = maxDiff;
        ctx.beginPath();
    },
    end: function () {
        this.ctx.stroke();
        this.ctx.restore();
    },
    endArea: function () {
        this.ctx.fill();
        this.ctx.restore();
    },
    getY: function (i) {
        var options = this.options; 
        var diff =options.getItemValue(this.data.items[i]) - options.middleValue;
        return 0 - diff * options.region.height / 2 / this.maxDiff; 
    },
    paintItem: function (i, x, y) {
        var ctx = this.ctx;

        if (i == 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    },
    paintArea: function (i, x, y , i1, x1 , y1) {
        var ctx = this.ctx;
        var options = this.options;
        var region = options.region;
        var grad = ctx.createLinearGradient(x,region.y/2 + region.height/2 -15,x1,y1);
				grad.addColorStop(0,'rgba('+parseInt(options.gradientColor.substring(1,3),16)+','+parseInt(options.gradientColor.substring(3,5),16)+','+parseInt(options.gradientColor.substring(5,7),16)+',0)');    
				grad.addColorStop(1,'rgba('+parseInt(options.gradientColor.substring(1,3),16)+','+parseInt(options.gradientColor.substring(3,5),16)+','+parseInt(options.gradientColor.substring(5,7),16)+',0.3)');
				ctx.fillStyle = grad;
        if (i == 0) {
        		ctx.moveTo(x,region.y/2 + region.height/2 -15);
            ctx.lineTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
        if(i == i1){
        		ctx.lineTo(x1,region.y/2 + region.height/2 -15);
        		ctx.fill();
        }
       	/*ctx.moveTo(x,region.y/2 + region.height/2 -15);
       	ctx.lineTo(x,y);
       	ctx.lineTo(x1,y1);
       	ctx.lineTo(x1,region.y/2 + region.height/2 -15);
       	ctx.fill();*/
    },
};