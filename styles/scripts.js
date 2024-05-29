function ecall() {
    if ($('.ecall').hasClass('collapse')) {
        document.querySelector('.ecall').textContent = 'Expand all'
        $('.ecall').removeClass('collapse').addClass('expand');
        $('ul.collapsibleList:visible').not('.dialogList').css({ 'display': 'none' });
        $('li.collapsibleListOpen').removeClass('collapsibleListOpen').addClass('collapsibleListClosed');
    } else {
        document.querySelector('.ecall').textContent = 'Collapse all'
        $('.ecall').removeClass('expand').addClass('collapse');
        $('ul.collapsibleList:hidden').css({ 'display': 'block' });
        $('li.collapsibleListClosed').removeClass('collapsibleListClosed').addClass('collapsibleListOpen');
    }
}

function shflags() {
    if (document.querySelector('.tags, .context, .checkflag, .setflag, .rolls, .approval, .ruletag').style.display  == '') {
        document.querySelector('.shflags').textContent = 'Show all flags'
        for (let el of document.querySelectorAll('.tags, .context, .checkflag, .setflag, .rolls, .approval, .ruletag')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shflags').textContent = 'Hide all flags'
        for (let el of document.querySelectorAll('.tags, .context, .checkflag, .setflag, .rolls, .approval, .ruletag')) el.style.display  = ''; 
    }
}

function shcontext() {
    if (document.querySelector('.context') === null) {
        alert("No context")
    } else if (document.querySelector('.context').style.display  == '') {
        document.querySelector('.shcontext').textContent = 'Show context'
        for (let el of document.querySelectorAll('.context')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shcontext').textContent = 'Hide context'
        for (let el of document.querySelectorAll('.context')) el.style.display  = ''; 
    }
}

function shtags() {
    if (document.querySelector('.tags') === null) {
        alert("No tags")
    } else if (document.querySelector('.tags').style.display  == '') {
        document.querySelector('.shtags').textContent = 'Show tags'
        for (let el of document.querySelectorAll('.tags')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shtags').textContent = 'Hide tags'
        for (let el of document.querySelectorAll('.tags')) el.style.display  = ''; 
    }
}

function shcheckflag() {
    if (document.querySelector('.checkflag') === null) {
        alert("No checkflags")
    } else if (document.querySelector('.checkflag').style.display  == '') {
        document.querySelector('.shcheckflag').textContent = 'Show checkflag'
        for (let el of document.querySelectorAll('.checkflag')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shcheckflag').textContent = 'Hide checkflag'
        for (let el of document.querySelectorAll('.checkflag')) el.style.display  = ''; 
    }
}

function shsetflag() {
    if (document.querySelector('.setflag') === null) {
        alert("No setflags")
    } else if (document.querySelector('.setflag').style.display  == '') {
        document.querySelector('.shsetflag').textContent = 'Show setflag'
        for (let el of document.querySelectorAll('.setflag')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shsetflag').textContent = 'Hide setflag'
        for (let el of document.querySelectorAll('.setflag')) el.style.display  = ''; 
    }
}

function shroll() {
    if (document.querySelector('.rolls') === null) {
        alert("No rolls")
    } else if (document.querySelector('.rolls').style.display  == '') {
        document.querySelector('.shroll').textContent = 'Show rolls'
        for (let el of document.querySelectorAll('.rolls')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shroll').textContent = 'Hide rolls'
        for (let el of document.querySelectorAll('.rolls')) el.style.display  = ''; 
    }
}

function shapprov() {
    if (document.querySelector('.approval') === null) {
        alert("No approvals")
    } else if (document.querySelector('.approval').style.display  == '') {
        document.querySelector('.shapprov').textContent = 'Show approval'
        for (let el of document.querySelectorAll('.approval')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shapprov').textContent = 'Hide approval'
        for (let el of document.querySelectorAll('.approval')) el.style.display  = ''; 
    }
}

function shrules() {
    if (document.querySelector('.ruletag') === null) {
        alert("No ruletags")
    } else if (document.querySelector('.ruletag').style.display  == '') {
        document.querySelector('.shrules').textContent = 'Show ruletag'
        for (let el of document.querySelectorAll('.ruletag')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shrules').textContent = 'Hide ruletag'
        for (let el of document.querySelectorAll('.ruletag')) el.style.display  = ''; 
    }
}

function shid() {
    if (document.querySelector('.nodeid').style.display  == '') {
        document.querySelector('.shid').textContent = 'Show node id'
        for (let el of document.querySelectorAll('.nodeid')) el.style.display  = 'none'; 
    } else  {
        document.querySelector('.shid').textContent = 'Hide node id'
        for (let el of document.querySelectorAll('.nodeid')) el.style.display  = ''; 
    }
}



$(document).ready(function() {
    CollapsibleLists.apply();

    $('span.goto').click(function (e) {
        var dataid = $(this).attr('data-id');
        if (dataid === undefined) {
            return;
        }
        var goto = $('#n' + dataid);
        if ($(goto).length == 0) {
            alert("Not found (id " + dataid + ")")
        }
        $(goto).parentsUntil('.dialogList').find('ul:hidden').show();
        $(this).show();
        $('html, body').animate({
            scrollTop: $(goto).offset().top
        }, 2000);
        $(goto).parent().css("background-color", "grey");

        setTimeout(function(){
            $(goto).parent().css("background", "transparent");
        }, 3000);

        $(goto).closest('.collapsibleListClosed').addClass('collapsibleListOpen').removeClass('collapsibleListClosed');
    });
});
