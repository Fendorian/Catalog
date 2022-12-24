

var GetLogin = function ()
{
    var username = $('#txtUsername').val();
    var password = $('#txtPassword').val();

    //var loginUrl = "/api/Login/GetLogin";
    var loginUrl = "http://10.114.10.21:88/api/Login/GetLogin";
    var loginData = JSON.stringify({ "Username": username, "Password": password });

    $.ajax({
        type: "POST",
        data: loginData,
        url: loginUrl,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result)
        {
            alert(result);
            if (result === "Logged in")
            {
                window.location.replace("http://10.114.10.21:82/");
            }
            else
            {
            }
            
        },
        error: function (result)
        {
            alert(result);
        },
    });

};