$(function () {

    $('#loginBlock').show();
    var toe = $.connection.toeHub;
    toe.client.onConnected = function (id, userName) {
        $('#hdId').val(id);
        $('#username').val(userName);
        $('#nick').html(userName);
        $('#loginBlock').hide();
        toe.server.updateUsers();
    }

    toe.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();
    }

    toe.client.Invite = function (user) {
        $('#confirm').html("");
        $("#confirm").append('<div class="row"><button id="GO' + user.UserId + '" class="btn btn-success col-sm-4 col-sm-offset-4">' + user.Name + ' invites you! </button></div>');
        $('#GO' + user.UserId).click(function () {
            toe.server.gamestart($('#hdId').val(), user.UserId);
            $('#GO' + user.UserId).remove();
        });
    }

    toe.client.DrawField = function () {
        $('.main').css("display", "none");
        $('.row').css("display", "none");
        $('#nick').css("display", "block");
        $('#body').css("display", "flex");
        $('.box').click(function () {
            toe.server.nextstep($('#hdId').val(), $(this).attr('id'));
        });
    }

    toe.client.STEP = function (pos, char) {
        if (char == 1) {
            $('#' + pos).text('X')
        }
        else {
            $('#' + pos).text('0')
        }
    }

    toe.client.WINNER = function (player) {
        alert(player.Name + "WINS!");
        $('.box').click(function () { });
    }

    toe.client.UpdateUsers = function (allUsers) {
        $("#users").html('');
        $("#users").append('<p><b>Players</b></p>');
        var userId = $('#hdId').val();
        for (var i = 0; i < allUsers.length; i++) {
            if (userId != allUsers[i].UserId) {
                $("#users").append('<div class="row"><button id="' + allUsers[i].UserId + '" class=" inv btn btn-warning col-sm-4 col-sm-offset-4">' + allUsers[i].Name + '</button></div>');
                $('#' + allUsers[i].UserId).click(function () {
                    toe.server.sendInvite($(this).attr('id'), userId);
                });
            }
        }
    }

    $.connection.hub.start().done(function () {
        toe.server.isLogged();
        $("#btnLogin").click(function () {

            var name = $("#txtUserName").val();
            if (name.length > 0) {
                toe.server.connect(name);
            }
            else {
                alert("Введите имя");
            }
        });

        $("#homebut").click(function () {
            $('.main').css("display", "block");
            $('.row').css("display", "block");
            $('#body').css("display", "none");
            $('.box').text("");
        });
    });
});

