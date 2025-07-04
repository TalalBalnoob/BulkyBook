let dataTable

$(document).ready(function () {
    loadDataTable()
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/Admin/Compony/GetAll/',
            dataSrc: ''
        },
        columns: [
            {data: "name", "width": "15%"},
            {data: "state", "width": "15%"},
            {data: "city", "width": "15%"},
            {data: "streetAddress", "width": "10%"},
            {data: "phoneNumber", "width": "15%"},
            {data: "id", "render": function (data) {
                return `<div class="w-75 btn-group" role="group">
                         <a href="/admin/compony/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Edit</a>
                         <a onclick=deleteProduct("/admin/compony/delete/${data}") class="btn btn-danger"><i class="bi bi-trash-fill"></i>Delete</a>
                      </div>`
                }, "width": "30%"
            }
        ]
    })
}

function deleteProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    dataTable.ajax.reload()
                }
            })
        }
    });
}
