function navHover() {
    let roos = this.getElementsByClassName("nav-roo");
    for (var i = 0; i < roos.length; i++) {
        roos.item(i).style.bottom = "100%";
    }
}

function navHoverLeave() {
    let roos = this.getElementsByClassName("nav-roo");
    for (var i = 0; i < roos.length; i++) {
        roos.item(i).style.bottom = "";
    }
}

function countdown(minutes, id) {
    var element = document.getElementById(id);
    var target = minutes * 60;
    setInterval(() => {
        var d = new Date();
        d.setTime(d.getTime() + (d.getTimezoneOffset() * 60000));
        var s = d.getHours() * 60 * 60 + d.getMinutes() * 60 + d.getSeconds();
        var i = target - s;
        if (i === 0) {
            location.reload();
        }
        if (i < 0) {
            i += 24 * 60 * 60;
        }
        var h = parseInt(i / (60 * 60));
        var m = parseInt((i / 60) % 60);
        var s = i % 60;

        m = m < 10 ? "0" + m : m;
        s = s < 10 ? "0" + s : s;

        element.innerHTML = h + ":" + m + ":" + s;
    }, 1000);
}

function toggleSidenav(e) {
    let elem = document.getElementsByClassName("sidenav")[0]
    if (elem.style.transform === "translate(100%)"){
      elem.style.transform = ""
  } else {
      elem.style.transform = "translate(100%)"
    }
  e.stopPropagation();
  return false;
}

$(".sidenav").click(e => {
    e.stopPropagation();
})

$(document).click(() => {
    let elem = document.getElementsByClassName("sidenav")[0]
    elem.style.transform = "translate(100%)"
})

function isScrolledIntoView(elem)
{
    var docViewTop = $(window).scrollTop();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var height = $(elem).height();
    var elemBottom = elemTop + $(elem).height();

    return (elemTop <= docViewBottom);
}

function Effects(){
    if ($(".header-text").length > 0 && isScrolledIntoView($(".header-text"))){
      $(".header-text").show("slide", { direction: "left" }, 1000);
    }
  $.each($(".vehicle"), (index, record) => {
    if (isScrolledIntoView($(record)) && $(record).css("opacity") === "0"){
      $(record).animate({opacity: "1"},{ duration: 500, queue: false });
    }
  })
}

$(window).scroll(() => {
  Effects();
}
);

$(window).on("load", () => {
    let navItems = document.getElementsByClassName("nav-item")
    for (var i = 0; i < navItems.length; i++) {
        navItems.item(i).addEventListener("mouseover", navHover)
        navItems.item(i).addEventListener("mouseout", navHoverLeave)
    }

    let sidenavToggler = document.getElementsByClassName("sidenav-toggler")[0]
    sidenavToggler.addEventListener("click", toggleSidenav)

    $.each($(".small-avatar"), (index, record) => {
        $(record).click(() => {
            if (!($(record).hasClass("rotated"))) {
                $(record).addClass("rotated");
                setTimeout(() => $(record).removeClass("rotated"), 2000);
            }
        })
    })
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
    if ($("#countdown").length > 0) {
        countdown(parseInt($("#countdown").attr("target")), "countdown");
    }
  Effects();
}
);