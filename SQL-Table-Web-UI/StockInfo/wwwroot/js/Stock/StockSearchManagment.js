var StockSearchManagment = {
        GetStocks: function () {
            $(document).ready(function () {
                $.ajax({
                    url: '/Home/Index',
                    //dataType: "json",
                    //method: 'post',
                    success: function (data) {
                        //var stockTable = $('#tblStock tbody');
                        //stockTable.empty();
                        //$(data).each(function (index, stk) {
                        //    stockTable.append('<tr><td>' + stk.itemId + '</td><td>'
                        //        + stk.name + '</td><td>' + stk.description + '</td></tr>');
                        //});
                    },
                    error: function (err) {
                        alert(err);
                    }
                });
            });  
    },
    //Below method will call the GetSearchResultsFunction via ajax, with the name variable
    //that is set via the search input on the index page. Tble variable is also passed which contains the 
    //initalisation of a Jquery Datatable

    GetStocksInfo: function (tble, name) {
        $(document).ready(function () {
            $.ajax({
                url: '/Home/GetSearchResults?name=' + name,
                dataType: "json",
                method: 'post',
                success: function (data) {

                    //below variables are set so that sorting will not change upon table refresh.
                    //note that due to the table refreshing every 1.25 seconds, on some occasions functionality
                    //of deleting table rows will take the user 2 clicks. This will be investigated further for solutions :)

                    tble.DataTable().destroy();
                    tble.DataTable({
                        data: data.html,
                        stateSave: true,
                        order: [],
                        bPaginate: false,
                        bLengthChange: false,
                        bFilter: true,
                        bInfo: false,
                        bAutoWidth: false,

                        "columns": [
                            { data: "itemId" },
                            { data: "name" },
                            { data: "description" },
                            {
                                //Add edit and delete buttons to each row record
                                "render": function (data, type, full) {

                                    return '<button class="btn btn-success" onclick="StockSearchManagment.GetStockInfo(' + full.itemId + ')">Edit</button><button class="btn btn-danger" onclick="StockSearchManagment.DeleteStock(' + full.itemId + ')">Delete</button>';

                                }

                            }
                        ]
                    });

                    

                },
                error: function (err) {
                    alert(err);
                }
            });
        });
    },

    //Below method is used to display the AddStock partial view  

    ShowAddStockUi: function () {
        $("#name").val("");
        $("#description").val("");
        $("#myModal").modal('show');
        $("#modal-title").html("Add Stock");
        $("#btnAddStock").attr("onClick", "StockSearchManagment.AddStock();");
        $("#btnAddStock").html("Add Stock");
    },

    //AddStock method below is used to pass the variables the user has entered
    //to the DatabaseRepository via ajax. 

    AddStock: function () {

        //Validation is performed on the users input so that no blank values 
        //can be entered.

        var name = $("#name").val();
        var description = $("#description").val();

        var message = "";
        if (name == "") {
            message = "Name is  required."
            $('#name').css('border-color', 'red');
        }
        if (description == "") {
            message += "Description is  required."
            $('#description').css('border-color', 'red');
        }

        if (message != "") {
            alert(message);
            return;
        }


        $.ajax({
            url: '/Home/AddStock',
            data: { 'name': name, 'description': description },
            success: (result) => {

                //once item is successfully added, hide the AddStock partial view
                $("#myModal").modal('hide');
            },
            error: (error) => {
                console.log(error)
            }
        });
    },

    //Below method is used to populate partial view with table row detials
    //When a user wishes to edit the table row. This is achieved by GetStockInfo being 
    //called from DatabaseRepository via ajax.

    GetStockInfo: function (ItemId) {
        $.ajax({
            url: '/Home/GetStockInfo',
            data: { 'itemId': ItemId },
            success: (result) => {
                if (result.success) {
                    $("#myModal").html(result.html);
                    $("#myModal").modal('show');
                    $("#btnAddStock").attr("onClick", "StockSearchManagment.UpdateStockInfo();");
                    $("#btnAddStock").html("Update Stock");
                    $("#modal-title").html("Update Stock");
                }
                else {
                    alert(result.errorMessage);
                }

            },
            error: (error) => {
                console.log(error)
            }
        });
    },

    //The below method is very similar to the AddStock method. 
    //variables the user has enetered when editing a table row is 
    //passed to the DatabaseRepository via ajax.

    UpdateStockInfo: function () {

        //Validation is performed on the users input so that no blank values 
        //can be entered.

        var name = $("#name").val();
        var description = $("#description").val();

        var message = "";
        if (name == "") {
            message = "Name is  required."
            $('#name').css('border-color', 'red');
        }
        if (description == "") {
            message += "Description is  required."
            $('#description').css('border-color', 'red');
        }

        if (message != "") {
            alert(message);
            return;
        }

        $.ajax({
            url: '/Home/UpdateStockInfo',
            data: { id: $("#itemId").val(), 'name': name, 'description': description },
            success: (result) => {
                if (result.success) {

                //once item is successfully edited, hide the AddStock partial view

                    StockSearchManagment.GetStocks();
                    $("#myModal").modal('hide');
                }
                else {
                    alert(result.errorMessage);
                }


            },
            error: (error) => {
                console.log(error)
            }
        });
    },

    //Lastly, the DeleteStock method will invoked from the DatabaseRepository.

    DeleteStock: function (itemId) {
        $.ajax({
            url: '/Home/DeleteStock',
            data: { 'id': itemId },
            success: function () {
            },
            error: (error) => {
                console.log(error)
            }

        });
    }

};