$(() => {
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            let response = await fetch(`api/employee`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(payload);

                msg === "" ? // are we appending to an existing message
                    $("#status").text("Employees Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            }
            else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
            else { // else 404 not found
                $("#status").text("no such path on server");
            }// else

            //Get department data
            response = await fetch(`api/department`);
            if (response.ok) {
                let depts = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("alldepartments", JSON.stringify(depts));
            }
            else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
            else { // else 404 not found
                $("#status").text("no such path on server");
            }// else
        }
        catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll function

    const buildEmployeeList = (data, useAllData = true) => {
        $("#employeeList").empty();
        div = $(` <div class="list-group-item text-white bg-primary row d-flex" id="status">Employee Info</div>
            <div class="list-group-item row d-flex text-center" id="heading">
                <div class="col-4 h4">Title</div>
                <div class="col-4 h4">First</div>
                <div class="col-4 h4">Last</div>
            </div>`);
        div.appendTo($("#employeeList"));
        sessionStorage.setItem("allEmployees", JSON.stringify(data));
        btn = $(`<button class="list-group-item row d-flex" id="0">
                    ...click to add employee
                    </button>`);
        btn.appendTo($("#employeeList"));
        useAllData ? sessionStorage.setItem("allEmployees", JSON.stringify(data)) : null;
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex" id="${emp.id}">`);
            btn.html(`<div class="col-4" id="employeetitle${emp.id}">${emp.title}</div>
                      <div class="col-4" id="employeefname${emp.id}">${emp.firstname}</div>
                      <div class="col-4" id="employeelastnam${emp.id}">${emp.lastname}</div>`
            );
            btn.appendTo($("#employeeList"));
        }); // for-each
    }; // buildEmployeeList function

    getAll(""); // first grab the data from the server

    document.addEventListener('keyup', e => {
        $("#modalstatus").removeClass(); // remove any existing css on div
        if ($("#EmployeeModalForm").valid()) {
            $("#actionbutton").prop("disabled", false);
        }
        else {
            $("#actionbutton").prop("disabled", true);
        }
    }); // keyup listener

    $("#EmployeeModalForm").validate({
        rules: {
            TextBoxTitle: { maxlength: 4, required: true, validTitle: true },
            TextBoxFirst: { maxlength: 25, required: true },
            TextBoxSurname: { maxlength: 25, required: true },
            TextBoxEmail: { maxlength: 40, required: true, email: true },
            TextBoxPhone: { maxlength: 15, required: true }
        },
        errorElement: "div",
        messages: {
            TextBoxTitle: {
                required: "required 1-4 characters.", maxlength: "required 1-4 characters.", validTitle: "Mr. Ms. Mrs. or Dr."
            },
            TextBoxFirst: {
                required: "required 1-25 characters.", maxlength: "required 1-25 characters."
            },
            TextBoxSurname: {
                required: "required 1-25 characters.", maxlength: "required 1-25 characters."
            },
            TextBoxPhone: {
                required: "required 1-15 characters.", maxlength: "required 1-15 characters."
            },
            TextBoxEmail: {
                required: "required 1-40 characters.", maxlength: "required 1-40 characters.", email: "Invalid email format."
            }
        }
    }); // EmployeeModalForm.validate

    $.validator.addMethod("validTitle", (value) => { // custom rule
        return (value === "Mr." ||
            value === "Ms." ||
            value === "Mrs." ||
            value === "Dr."
        );
    }, ""
    ); // .validator.addMethod()

  

    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        }// clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allEmployees"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        }
        else {
            return false; // ignore if they clicked on heading or status
        }
    });// employeeListClick

    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "update" ? update() : add();
    });// actionbutton click

    $("#deletebutton").on("click", () => {
        $("#dialog").show();
    }); // deletebutton click

    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
    });

    $("#yesbutton").on("click", (e) => {
        $("#dialog").hide();
        $("#actionbutton").show();
        _delete();
    });

    $("#generatePdf").on("click", async (e) => {
        try {
            $("#status").text("generating report on server - please wait...");
            let response = await fetch(`api/employeereport`);
            if (!response.ok) {
                throw new Error(`Status = ${response.status}, Text - ${response.statusText}`);
            }

            let data = await response.json(); // promise - await it.
            $("#status").text("Report generated");
            data.msg === "Report Generated" ?
                window.open("/pdfs/employeereport.pdf") :
                $("#status").text("Problem generating report :(");
        }
        catch (error) {
            $("#status").text(error.message);
        } // try-catch
    }); // report button click

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allEmployees"));
        let filteredData = alldata.filter((emp) => emp.lastname.match(new RegExp($("#srch").val(), 'i')));
        buildEmployeeList(filteredData, false);
    });

    const add = async () => {
        try {
            emp = new Object();
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirst").val();
            emp.lastname = $("#TextBoxSurname").val();
            emp.email = $("#TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();

            emp.departmentId = parseInt($("#ddlDepartments").val()); // <- long awaited dropdown
            emp.id = -1;
            emp.timer = null;
            emp.picture64 = null;
            // send the employee info to the server asynchronously using POST
            let response = await fetch("api/employee", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify(emp)
            });

            if (response.ok) { // or check for response.status
                let data = await response.json();
                getAll(data.msg);
            }
            else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
            else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        }
        catch (error) {
            $("#status").text(error.message);
        }// try-catch
        $("#theModal").modal("toggle");
    };// add

    const _delete = async () => {
        let employee = JSON.parse(sessionStorage.getItem("employee"));
        try {
            let response = await fetch(`api/employee/${employee.id}`, {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json; charset=utf-8' }
            });
            if (response.ok) // or check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            }
            else {
                $('#status').text(`Status - ${response.status}, Problem on delete server side, see server console`);
            } // else
            $('#theModal').modal('toggle');
        } catch (error) {
            $('#status').text(error.message);
        }
    }; // _delete

    const update = async (e) => {
        // update button click event handler
        try {
            // set up a new client side instance of Employee
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            // pouplate the properties
            emp.title = $("#TextBoxTitle").val();
            emp.firstname = $("#TextBoxFirst").val();
            emp.lastname = $("#TextBoxSurname").val();
            emp.email = $("#TextBoxEmail").val();
            emp.phoneno = $("#TextBoxPhone").val();
            emp.departmentId = parseInt($("#ddlDepartments").val());

            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                getAll(payload.msg);

            } else if (response.status !== 404) {
                // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        } // try/catch
        $("#theModal").modal("toggle");
    }; // update

    const loadDepartmentDDL = (empdept) => {
        html = '';
        $("#ddlDepartments").empty();
        let allDepartments = JSON.parse(sessionStorage.getItem("alldepartments"));
        allDepartments.forEach((dept) => { html += `<option value="${dept.id}">${dept.name}</option>` });
        $("#ddlDepartments").append(html);
        $("#ddlDepartments").val(empdept);
    }; // loadDepartmentDDL 

    const clearModalFields = () => {
        loadDepartmentDDL(-1);
        $("#TextBoxTitle").val("");
        $("#TextBoxFirst").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPhone").val("");
        sessionStorage.removeItem("employee");
        sessionStorage.removeItem("picture");
        $("#uploadstatus").text("");
        $("#imageHolder").html("");
        $("#uploader").val("");
        $("#theModal").modal("toggle");
        let validator = $("#EmployeeModalForm").validate();
        validator.resetForm();

    }; // clearModalFields

    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>Add Employee</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("Add New Employee");
        $("#theModalLabel").text("Add");
        $("#deletebutton").hide();
        $("#actionbutton").show();
        $("#dialog").hide();
        let validator = $("#EmployeeModalForm").validate();
        validator.resetForm();
        clearModalFields();
    }; // setupForAdd

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>Update Employee</h4>");
        $("#cardheader").html("Update Employee");
        clearModalFields();
        data.forEach(employee => {
            if (employee.id === parseInt(id)) {
                $("#TextBoxTitle").val(employee.title);
                $("#TextBoxFirst").val(employee.firstname);
                $("#TextBoxSurname").val(employee.lastname);
                $("#TextBoxEmail").val(employee.email);
                $("#TextBoxPhone").val(employee.phoneno);

                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
                loadDepartmentDDL(employee.departmentId);
                $("#imageHolder").html(`<img height="120" width="110" src="data:img/png;base64,${employee.staffPicture64}" />`);
            } // if
        }); // data.forEach

        $("#deletebutton").show();
        $("#dialog").hide();
    };

    $("input:file").on("change", () => {
        try {
            const reader = new FileReader();
            const file = $("#uploader")[0].files[0];
            $("#uploadstatus").text("");
            file ? reader.readAsBinaryString(file) : null;
            reader.onload = (readerEvt) => {
                // get binary data then convert to encoded string
                const binaryString = reader.result;
                const encodedString = btoa(binaryString);
                // replace the picture in session storage
                let employee = JSON.parse(sessionStorage.getItem("employee"));
                employee.staffPicture64 = encodedString;
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#uploadstatus").text("retrieved local pic")
            };
        } catch (error) {
            $("#uploadstatus").text("pic upload failed")
        }
    }); // input file change



}); // jQuery ready function

// server was reached but server had a problem with the call
const errorRtn = (problemJson, status) => {
    if (status > 499) {
        $("#status").text("Problem server side, see debug console");
    }
    else {
        let keys = Object.keys(problemJson.errors)
        problem = {
            status: status,
            statusText: problemJson.errors[keys[0]][0], // first error
        };
        $("#status").text("Problem client side, see browser console");
        console.log(problem);
    } // else
}