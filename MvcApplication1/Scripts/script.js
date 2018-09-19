<script type="text/javascript">

    var sec1 = 20;
    var t1;

    function d1(x1) {
        sec1 = x1;

        n1();

    }
    function p1() {
        if (sec1 > 9) {
            document.getElementById("new").innerHTML = "00 : " + sec1;

            sec1--;
        }
        else {
            document.getElementById("new").innerHTML = "00 : 0" + sec1;

            sec1--;
        }
        if (sec1 < 0) {
            clearInterval(t1);

        }
    }

    function n1() {
        t1 = setInterval("p1()", 1000);
    }
                       </script>
