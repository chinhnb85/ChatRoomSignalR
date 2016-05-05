$(function() {
    //created gif from video
    //gifshot.createGIF({
    //    gifWidth: 300,
    //    gifHeight: 300,
    //    video: [
    //        '/Modules/GifShot/videos/example.mp4',
    //        '/Modules/GifShot/videos/example.ogv'
    //    ],
    //    interval: 0.2,
    //    numFrames: 24,
    //    text: 'Hello',
    //    fontWeight: 'bold',
    //    fontFamily: 'Arial',
    //    fontColor: '#ff0000',
    //    textAlign: 'left',
    //    textBaseline: 'top',
    //    sampleInterval: 15,
    //    numWorkers: 3
    //}, function(obj) {
    //    if (!obj.error) {
    //        var image = obj.image, animatedImage = document.createElement('img');
    //        animatedImage.src = image;
    //        document.body.appendChild(animatedImage);
    //    }
    //});

    //created gif from images
    var imgs = [];

    //for (var i = 1; i < 4; i++) {
    //    var link = String.format("http://dummyimage.com/150x{1}/{0}{0}{0}/fff.png&text={2}", i, i * 50, i + 1);
    //    imgs.push(link);
    //}

    imgs.push("/Modules/GifShot/img/2OO33vX.jpg");
    imgs.push("/Modules/GifShot/img/qOwVaSN.png");
    imgs.push("/Modules/GifShot/img/Vo5mFZJ.gif");

    gifshot.createGIF({
        //images: [
        //    '/Modules/GifShot/img/2OO33vX.jpg',
        //    '/Modules/GifShot/img/qOwVaSN.png',
        //    '/Modules/GifShot/img/Vo5mFZJ.gif'],
        images:imgs,
        interval: 0.2,
        text: '#Hello',
        fontWeight: 'bold',        
        fontColor: '#333',
        textAlign: 'center',
        textBaseline: 'bottom',
        gifWidth: 400,
        gifHeight: 300,
        crossOrigin: '',
        progressCallback: function(captureProgress) {
            var ep = document.createElement('p');
            var t = document.createTextNode("Loading...");
            ep.appendChild(t);
            document.body.appendChild(ep);
        },
        completeCallback: function() {
            
        }
    }, function (obj) {
        if (!obj.error) {
            var image = obj.image,
            animatedImage = document.createElement('img');
            animatedImage.src = image;
            document.body.appendChild(animatedImage);
            $('p').remove();
        }
    });

    //created gif from web cam
    //gifshot.createGIF(function (obj) {
    //    if (!obj.error) {
    //        var image = obj.image,
    //        animatedImage = document.createElement('img');
    //        animatedImage.src = image;
    //        document.body.appendChild(animatedImage);
    //    }
    //});

    //type snap shot
    //gifshot.takeSnapShot(function (obj) {
    //    if (!obj.error) {
    //        var image = obj.image,
    //        animatedImage = document.createElement('img');
    //        animatedImage.src = image;
    //        document.body.appendChild(animatedImage);
    //    }
    //});
});

String.format = function() {
    // The string containing the format items (e.g. "{0}")
    // will and always has to be the first argument.
    var theString = arguments[0];

    // start with the second argument (i = 1)
    for (var i = 1; i < arguments.length; i++) {
        // "gm" = RegEx options for Global search (more than one instance)
        // and for Multiline search
        var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
        theString = theString.replace(regEx, arguments[i]);
    }
    return theString;
}