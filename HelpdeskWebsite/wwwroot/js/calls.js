$(() => {
    const getAll = async (msg) => {
        try {
            $("#callList").text("Finding Call Information...");
            let response = await fetch(`api/call`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("allCalls", JSON.stringify(payload));
                buildCallList(payload);
                msg === "" ? // are we appending to an existing message
                    $("#status").text("Calls Loaded") : $("#status").text(`${msg} - Calls Loaded`);
            }
            else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
            else { // else 404 not found
                $("#status").text("no such path on server");
            }// else

            //Get employee data
            response = await fetch(`api/employee`);
            if (response.ok) {
                let emps = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("allEmployees", JSON.stringify(emps));
            }
            else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
            else { // else 404 not found
                $("#status").text("no such path on server");
            }// else

            //Get problem data
            response = await fetch(`api/problem`);
            if (response.ok) {
                let depts = await response.json(); // this returns a promise, so we await it
                sessionStorage.setItem("allProblems", JSON.stringify(depts));
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

    const buildCallList = (data) => {
        $("#callList").empty();
        div = $(` <div class="list-group-item text-white bg-primary row d-flex" id="status">Call Info</div>
            <div class="list-group-item row d-flex text-center" id="heading">
                <div class="col-4 h4">Date</div>
                <div class="col-4 h4">For</div>
                <div class="col-4 h4">Problem</div>
            </div>`);
        div.appendTo($("#callList"));

        btn = $(`<button class="list-group-item row d-flex" id="0">
                    ...click to add call
                    </button>`);
        btn.appendTo($("#callList"));

        data.forEach(call => {
            btn = $(`<button class="list-group-item row d-flex" id="${call.id}">`);
            btn.html(`<div class="col-4" id="callopened${call.id}">${formatDate(call.dateOpened).replace("T", " ")}</div>
                      <div class="col-4" id="callemployee${call.id}">${call.employeeName}</div>
                      <div class="col-4" id="callproblem${call.id}">${call.problemDescription}</div>`
            );
            btn.appendTo($("#callList"));
        }); // for-each
    }; // buildCallList function

    getAll(""); // first grab the data from the server

    document.addEventListener('keyup', e => {
        $("#modalstatus").removeClass(); // remove any existing css on div
        if ($("#CallModalForm").valid()) {
            $("#actionbutton").prop("disabled", false);
        }
        else {
            $("#actionbutton").prop("disabled", true);
        }
    }); // keyup listener

    $("#CallModalForm").validate({
        rules: {
            ddlProblems: { required: true},
            ddlEmployees: { required: true },
            ddlTechs: { required: true },
            TextBoxNotes: { maxlength: 250, required: true }
        },
        errorElement: "div",
        messages: {
            ddlProblems: { required: "Select a Problem" },
            ddlEmployees: { required: "Select an Employee" },
            ddlTechs: { required: "Select a Technician" },
            TextBoxNotes: {
                required: "Required: 1-250 characters!",
                maxlength: "Required: 1-250 characters!"
            }
        }
    }); // CallModalForm.validate

    $("#callList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "callList" || id === "") {
            id = e.target.id;
        }// clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allCalls"));
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        }
        else {
            return false; // ignore if they clicked on heading or status
        }
    });// callList click

    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "Update" ? update() : add();
    });// actionbutton click

    $("#deletebutton").on("click", () => {
        $("#dialog").show();
    }); // deletebutton click

    $("#nobutton").on("click", (e) => {
        $("#dialog").hide();
    }); // nobutton click

    $("#yesbutton").on("click", (e) => {
        $("#dialog").hide();
        $("#actionbutton").show();
        _delete();
    }); // yesbutton click

    $("#srch").on("keyup", () => {
        let allCalls = JSON.parse(sessionStorage.getItem("allCalls"));
        let filteredData = allCalls.filter((call) => call.employeeName.match(new RegExp($("#srch").val(), 'i')));
        buildCallList(filteredData);
    }); // search bar query handler

    $("#checkBoxClose").on("click", () => {
        if ($("#checkBoxClose").is(":checked")) {
            $("#dateClosed").text(formatDate().replace("T", " "));
            sessionStorage.setItem("dateClosed", formatDate());
        } else {
            $("#labelDateClosed").text("");
            sessionStorage.setItem("dateClosed", "");
        }
    }); // checkBoxClose

    const add = async () => {
        try {
            call = new Object();
            call.problemId = parseInt($("#ddlProblems").val());
            call.employeeId = parseInt($("#ddlEmployees").val());
            call.techId = parseInt($("#ddlTechs").val());
            call.dateOpened = formatDate(sessionStorage.getItem("dateOpened"));
            call.notes = $("#TextBoxNotes").val();
            call.openStatus = true;
            call.id = -1;
            call.timer = null;

            // send the call info to the server asynchronously using POST
            let response = await fetch("api/Call", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify(call)
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
        let call = JSON.parse(sessionStorage.getItem("call"));
        try {
            let response = await fetch(`api/call/${call.id}`, {
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
            // set up a new client side instance of Call
            let call = JSON.parse(sessionStorage.getItem("call"));
            // pouplate the properties
            call.problemId = parseInt($("#ddlProblems").val());
            call.employeeId = parseInt($("#ddlEmployees").val());
            call.techId = parseInt($("#ddlTechs").val());
            //call.dateOpened = formatDate(sessionStorage.getItem("dateOpened"));
            let dateC = sessionStorage.getItem("dateClosed");
            if (dateC === "") {
                call.dateClosed = null;
                call.openStatus = true;
            }
            else {
                call.dateClosed = formatDate(dateC);
                call.openStatus = false;    
            }

            call.notes = $("#TextBoxNotes").val();
            call.timer = JSON.parse(sessionStorage.getItem("call")).timer;
            
            // send the updated call back to the server asynchronously using Http-PUT
            let response = await fetch("api/call", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(call),
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

    const loadProblemDDL = (problem) => {
        phtml = "";
        $("#ddlProblems").empty();
        let allProblems = JSON.parse(sessionStorage.getItem("allProblems"));
        allProblems.forEach((p) => { phtml += `<option value="${p.id}">${p.description}</option>` });
        $("#ddlProblems").append(phtml);
        $("#ddlProblems").val(problem);
    }; //loadProblemDDL

    const loadEmployeesDDL = (employee) => {
        ehtml = "";
        $("#ddlEmployees").empty();
        let allEmployees = JSON.parse(sessionStorage.getItem("allEmployees"));
        allEmployees.forEach((e) => { ehtml += `<option value="${e.id}">${e.lastname}</option>` });
        $("#ddlEmployees").append(ehtml);
        $("#ddlEmployees").val(employee);
    }; //loadEmployeesAndTechsDDL

    const loadTechDDL = (techn) => {
        thtml = "";
        $("#ddlTechs").empty();
        let allEmployees = JSON.parse(sessionStorage.getItem("allEmployees"));
        let allTechs = allEmployees.filter(t => t.isTech === true);
        allTechs.forEach((t) => { thtml += `<option value="${t.id}">${t.lastname}</option>` });
        $("#ddlTechs").append(thtml);
        $("#ddlTechs").val(techn);
    } //loadTechDDL

    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) { _day = "0" + _day; }
        let _month = d.getMonth() + 1;
        if (_month < 10) { _month = "0" + _month; }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) { _hour = "0" + _hour; }
        let _min = d.getMinutes();
        if (_min < 10) { _min = "0" + _min; }
        return _year + "-" + _month + "-" + _day + "T" + _hour + ":" + _min;
    } // formatDate

    const clearModalFields = () => {
        loadProblemDDL(-1);
        loadEmployeesDDL(-1);
        loadTechDDL(-1);
        
        $("#ddlProblems").attr("disabled", false);
        $("#ddlEmployees").attr("disabled", false);
        $("#ddlTechs").attr("disabled", false);
        $("#TextBoxNotes").attr("disabled", false);
        $("#TextBoxNotes").val("");

        $("#theModal").modal("toggle");
        let validator = $("#CallModalForm").validate();
        validator.resetForm();

    }; // clearModalFields

    const setupForAdd = () => {

        $("#actionbutton").val("Add");
        $("#deletebutton").hide();
        $("#actionbutton").show();

        $("#modalstatus").text("Add New Call");
        $("#theModalLabel").text("Add");
        $("#dialog").hide();
        $("#theModal").modal("toggle");
        $("#dateOpened").text(formatDate().replace("T", " "));
        $("#dateClosed").hide();
        $("#dateClosedLabel").hide();
        $("#checkBoxLabel").hide();
        $("#checkBoxDiv").hide();
        sessionStorage.setItem("dateOpened", formatDate());
        let validator = $("#CallModalForm").validate();
        validator.resetForm();
        clearModalFields();
    }; // setupForAdd

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("Update");
        clearModalFields();
        data.forEach(call => {
            if (call.id === parseInt(id)) {
                $("#TextBoxNotes").val(call.notes);
                $("#dateOpened").text(formatDate(call.dateOpened).replace("T", " "));
                $("#dateClosed").text("");
                $("#dateClosedLabel").show();
                $("#dateClosed").show();
                sessionStorage.setItem("call", JSON.stringify(call));
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
                loadProblemDDL(call.problemId);
                loadEmployeesDDL(call.employeeId);
                loadTechDDL(call.techId);
                $("#checkBoxLabel").show();
                $("#checkBoxDiv").show();
                if (!call.openStatus) {
                    // call is closed so data is read only and Update button is hidden
                    $("#dateClosed").text(formatDate(call.dateClosed).replace("T", " "));
                    $("#ddlProblems").attr("disabled", true);
                    $("#ddlEmployees").attr("disabled", true);
                    $("#ddlTechs").attr("disabled", true);
                    $("#TextBoxNotes").attr("disabled", true);
                    $("#checkBoxClose").attr("disabled", true);
                    $("#checkBoxClose").prop("checked", true);

                    $("#actionbutton").hide();
                }
                else {
                    $("#checkBoxClose").attr("disabled", false);
                    $("#checkBoxClose").prop("checked", false);
                    $("#actionbutton").show();
                }
            } // if
            
        }); // data.forEach
        
        $("#deletebutton").show();
        $("#dialog").hide();
    };

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