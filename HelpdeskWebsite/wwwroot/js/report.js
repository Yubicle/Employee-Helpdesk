$(() => {

    $("#empReport").on("click", async (e) => {
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
    }); // employeeReport button click

    $("#callReport").on("click", async (e) => {
        try {
            $("#status").text("generating report on server - please wait...");
            let response = await fetch(`api/callreport`);
            if (!response.ok) {
                throw new Error(`Status = ${response.status}, Text - ${response.statusText}`);
            }

            let data = await response.json(); // promise - await it.
            $("#status").text("Report generated");
            data.msg === "Report Generated" ?
                window.open("/pdfs/callreport.pdf") :
                $("#status").text("Problem generating report :(");
        }
        catch (error) {
            $("#status").text(error.message);
        } // try-catch
    }); // callReport button click
});