let dataTable

$(document).ready(function () {
    var url = window.location.search;
    
    if (url.includes("inprocess")) loadDataTable("inprocess")
    else if (url.includes("pending")) loadDataTable("pending")
    else if (url.includes("complete")) loadDataTable("complete")
    else if (url.includes("approved")) loadDataTable("approved")
    else loadDataTable("all")
    
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: 'order/getall?status=' + status,
            dataSrc: ''
        },
        columns: [
            {data: "id", "width": "10%"},
            {data: "name", "width": "15%"},
            {data: "phoneNumber", "width": "10%"},
            {data: "user.email", "width": "15%"},
            {data: "orderStatus", "width": "15%"},
            {data: "orderTotal", "width": "20i%"},
            {data: "id", "render": function (data) {
                return `<div class="w-75 btn-group" role="group">
                         <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-info-circle-fill"></i>Details</a>
                      </div>`
                }, "width": "15%"
            }
        ]
    })
}


