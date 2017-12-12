var baseUrl = "https://lts-api.com/sift-voting/1/";
var apiCallXhr = null;
var referendum = null;
var id = null;
var configurePostApiSuccess = null;

function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split('=');
        if (decodeURIComponent(pair[0]) === variable)
            return decodeURIComponent(pair[1]);
    }
    return null;
}

function isInt(value) {
    var x;
    if (isNaN(value)) {
        return false;
    }
    x = parseFloat(value);
    return (x | 0) === x;
}


function indexPageLoad() {
    // Disable the UI
    document.getElementById("disabledOverlay").style.display = "block";

    // We want to get a list of all votes here
    apiCallXhr = new XMLHttpRequest();
    apiCallXhr.open("GET", baseUrl + "referendum", true);
    apiCallXhr.onload = onApiSuccess;
    apiCallXhr.onerror = onApiError;
    apiCallXhr.onabort = onApiAbort;
    apiCallXhr.ontimeout = onApiTimeout;
    configurePostApiSuccess = function (json) {
        apiCallXhr = null;
        var responseObject = JSON.parse(json);
        var voteList = document.getElementById("voteList");
        for (var i = 0; i < responseObject.length; i++) {
            var li = document.createElement("li");
            referendum = responseObject[i];
            li.innerHTML = '<a href="referendum.html?id=' + referendum.Id + '">' + referendum.Question + '</a>';
            voteList.appendChild(li);
        }
        document.getElementById("disabledOverlay").style.display = "none";
    }
    apiCallXhr.send();
}


function votePageLoad() {
    // Check we have ID
    id = getQueryVariable("id");
    if (!id) {
        showErrors(['You must specify which referendum you wish to see']);
        return;
    }
    if (!isInt(id)) {
        showErrors(['The specified referendum ID is not valid']);
        return;
    }

    // Disable the UI
    document.getElementById("disabledOverlay").style.display = "block";

    // We want to get a list of all votes here
    apiCallXhr = new XMLHttpRequest();
    apiCallXhr.open("GET", baseUrl + "referendum/" + id, true);
    apiCallXhr.onload = onApiSuccess;
    apiCallXhr.onerror = onApiError;
    apiCallXhr.onabort = onApiAbort;
    apiCallXhr.ontimeout = onApiTimeout;
    configurePostApiSuccess = function (json) {
        apiCallXhr = null;

        // Get a handle to the referendum
        referendum = JSON.parse(json);
        if (!referendum) {
            showErrors(['No referendum could be obtained from our backend systems']);
            return;
        }
        document.getElementById('voteSection').style.display = null;

        // First, let's determine whether the vote is open, closed or not yet open and set visibilities accordingly
        var openTime = Date.parse(referendum.StartTime);
        var closeTime = Date.parse(referendum.EndTime);
        var now = new Date().getTime();
        if (now < openTime) {
            document.getElementById('votingNotOpenSection').style.display = null;
            document.getElementById('voteOpensTime').innerText = referendum.StartTime;
        }
        else if (now > closeTime) {
            document.getElementById('votingClosedSection').style.display = null;
            document.getElementById('voteEndedTime').innerText = referendum.EndTime;
            document.getElementById('resultsContainer').style.display = null;
        }
        else {
            document.getElementById('castVoteSection').style.display = null;
            document.getElementById('resultsContainer').style.display = null;
        }

        // Setup the vote details
        document.getElementById('questionHeader').innerText = referendum.Question;

        // Add the answers to the drop down
        var voteAnswer = document.getElementById('voteAnswer');
        for (var i = 0; i < referendum.Answers.length; i++) {
            var option = document.createElement("option");
            option.value = (i + 1);
            option.innerText = referendum.Answers[i];
            voteAnswer.appendChild(option);
        }
        voteAnswer.selectedIndex = -1;

        // Update results pane
        recalculateResults();

        // Update voter list
        recalculateVoters();

        // Reenable the UI
        document.getElementById("disabledOverlay").style.display = "none";
    }
    apiCallXhr.send();
}

function recalculateMessageToSign() {
    var msg = '';
    var index = document.getElementById('voteAnswer').selectedIndex;
    var address = document.getElementById('voteAddress').value;
    if (index >= 0 && id && address)
        msg = 'R' + id + ' V' + (index + 1) + ' ' + address;
    document.getElementById('messageToSign').value = msg;
}

function copyMessageToSign() {
    recalculateMessageToSign();
    var messageToSign = document.getElementById('messageToSign');
    messageToSign.select();
    document.execCommand("Copy");
}

function copySignature(signature) {
    var copySignatureInput = document.getElementById('copySignatureInput');
    copySignatureInput.style.display = null;
    copySignatureInput.value = signature;
    copySignatureInput.select();
    document.execCommand("Copy");
    copySignatureInput.style.display = 'none';
}

function onApiSuccess(e) {
    // Ignore non-ready states
    if (apiCallXhr.readyState !== 4)
        return;

    // Check for 400 response - bad data and show validators
    if (apiCallXhr.status === 200 || apiCallXhr.status === 204) {
        configurePostApiSuccess(apiCallXhr.responseText);
    } else if (apiCallXhr.status === 400) {
        configurePostApiValidationError(apiCallXhr.responseText);
    } else if (apiCallXhr.status === 404) {
        configurePostApiError(['The specified referendum could not be found']);
    } else {
        // Log the error in the console and display error section
        console.error("Unexpected status code: " + apiCallXhr.status);
        configurePostApiError("There was an error talking to our backend systems please contact support quoting status code " + apiCallXhr.status + ".");
    }
}

function onApiError(e) {
    // Log the error in the console and display error section
    console.error("API request failed: " + apiCallXhr.statusText);
    configurePostApiError("There was an error whilst attempting to communicate with our backend systems, please try again.");
}

function onApiTimeout(e) {
    // Log the error in the console and display error section
    console.error("API request timed out: " + apiCallXhr.statusText);
    configurePostApiError("The connection to our backend systems timed out, please try again.");
}

function onApiAbort(e) {
    // Log the error in the console and display error section
    console.error("API request aborted: " + apiCallXhr.statusText);
    configurePostApiError("The connection to our backend systems was interrupted, please try again.");
}

function configurePostApiError(message) {
    // Update  UI to include this error and re-enable UI
    showErrors([message]);
    document.getElementById("disabledOverlay").style.display = "none";

    // Mark that we are done with this request
    apiCallXhr = null;
}

function configurePostApiValidationError(json) {
    // Our response object includes invalid fields which we must set to be in an error state as well as
    // error messages
    var validationResponse = JSON.parse(json);
    if (validationResponse.InvalidFields)
        for (var i = 0; i < validationResponse.InvalidFields.length; i++)
            document.getElementById(validationResponse.InvalidFields[i]).classList.add("validation-error");
    showErrors(validationResponse.ErrorMessages);

    // Enable the UI
    document.getElementById("disabledOverlay").style.display = "none";

    // Mark that we are done with this request
    apiCallXhr = null;
}
function showErrors(errors) {
    // Return if no errors
    if (!errors || errors.length === 0)
        return;

    // Show the error details
    var errorList = document.getElementById("errorList");
    errorList.innerHTML = "";
    for (var i = 0; i < errors.length; i++) {
        var li = document.createElement("li");
        li.style.fontSize = "14px";
        li.innerHTML = errors[i];
        errorList.appendChild(li);
    }
    document.getElementById("errorContainer").style.display = null;
}

function recalculateResults() {
    // Count total votes cast
    var totalCast = 0;
    var votes = [];
    var i;
    for (i = 0; i < referendum.Answers.length; i++)
        votes.push(0);
    for (i = 0; i < referendum.Electorate.length; i++) {
        var voter = referendum.Electorate[i];
        if (voter.Vote < 1)
            continue;
        totalCast += voter.VoteCount;
        votes[voter.Vote - 1] += voter.VoteCount;
    }

    // Add the results pane
    var results = document.getElementById('results');
    results.innerHTML = null;
    for (i = 0; i < referendum.Answers.length; i++) {
        // Get voting results so far for this vote
        var voteCount = votes[i];
        var percentage = totalCast === 0 ? '0%' : ((voteCount / totalCast) * 100).toFixed(1) + '%';

        // Create the HTML for the voting percentage bar
        var p = document.createElement("p");
        var labelText = referendum.Answers[i];
        if (voteCount === 0)
            labelText = referendum.Answers[i] + ' (no votes)';
        else if (voteCount === 1)
            labelText = referendum.Answers[i] + ' (1 vote)';
        else
            labelText = referendum.Answers[i] + ' (' + voteCount + ' votes)';
        p.innerText = labelText;
        results.appendChild(p);
        var outerDiv = document.createElement("div");
        outerDiv.className = "percentage-background";
        p.appendChild(outerDiv);
        var innerDiv = document.createElement("div");
        innerDiv.className = "percentage-inside";
        innerDiv.style.width = percentage;
        var span = document.createElement("span");
        span.innerText = percentage;
        innerDiv.appendChild(span);
        outerDiv.appendChild(innerDiv);
    }
}

function recalculateVoters() {
    // Clear the table first
    var voterTable = document.getElementById('voterTable').getElementsByTagName('tbody')[0];;
    var i;
    for (i = voterTable.rows.length - 1; i >= 0; i--)
        voterTable.deleteRow(i);

    // Add the voters
    var showCastOnly = document.getElementById('showCastOnly').checked;
    for (i = 0; i < referendum.Electorate.length; i++) {
        // Skip this voter if they haven't voted and we've got show cast only enabled
        var voter = referendum.Electorate[i];
        if (voter.Vote < 1 && showCastOnly)
            continue;
        var vote = voter.Vote === 0 ? ' ' : referendum.Answers[voter.Vote - 1];

        // Add the voter data
        var row = voterTable.insertRow(voterTable.rows.length);
        var addressCell = row.insertCell(0);
        var countCell = row.insertCell(1);
        var voteCell = row.insertCell(2);
        var signatureCell = row.insertCell(3);
        addressCell.innerText = voter.Address;
        countCell.innerText = voter.VoteCount;
        voteCell.innerText = vote;

        // Add signature data
        var message = 'R' + id + ' V' + voter.Vote + ' ' + voter.Address;
        signatureCell.innerHTML = voter.Vote === 0 ? '' : '<a href="javascript:copySignature(\'' + voter.SignedVoteMessage + '\');">copy signature</a> &nbsp; <a href="javascript:copySignature(\'' + message + '\');">copy message</a>';
    }
}

function performVote() {
    // Clear validation first
    document.getElementById('voteAddress').classList.remove('validation-error');
    document.getElementById('voteAnswer').classList.remove('validation-error');
    document.getElementById('signature').classList.remove('validation-error');
    document.getElementById("errorContainer").style.display = 'none';

    // Perform whatever client-side validation that we can 
    var errors = [];
    var address = document.getElementById('voteAddress').value;
    var answer = document.getElementById('voteAnswer').selectedIndex + 1;
    var signature = document.getElementById('signature').value;
    if (!address) {
        document.getElementById('voteAddress').classList.add('validation-error');
        errors.push('Your Ethereum address is required');
    }
    else if (address.length !== 42) {
        document.getElementById('voteAddress').classList.add('validation-error');
        errors.push('Your Ethereum address is not valid');
    }
    else {
        var found = false;
        for (var i = 0; i < referendum.Electorate.length; i++) {
            var voter = referendum.Electorate[i];
            if (voter.Address.toLowerCase() !== address.toLowerCase())
                continue;
            if (voter.VoteCount === 0)
                continue;
            found = true;
            break;
        }
        if (!found) {
            document.getElementById('voteAddress').classList.add('validation-error');
            errors.push('Your Ethereum address is not entitled to vote');
        }
    }
    if (!answer || answer < 1) {
        document.getElementById('voteAnswer').classList.add('validation-error');
        errors.push('Your need to select your voting intention');
    }
    if (!signature) {
        document.getElementById('signature').classList.add('validation-error');
        errors.push('Your need to sign your vote');
    }
    else if (!signature.startsWith("0x")) {
        // Here we need to try and parse the MEW response out
        try {
            var mewSignature = JSON.parse(signature);
            signature = mewSignature.sig;
            if (!signature) {
                document.getElementById('signature').classList.add('validation-error');
                errors.push('Your signature must be in either MyEtherWallet or hex format');
            }
        } catch (e) {
            signature = '';
            document.getElementById('signature').classList.add('validation-error');
            errors.push('Your signature must be in either MyEtherWallet or hex format');
        }
    }
    if (signature && signature.length !== 132) {
        document.getElementById('signature').classList.add('validation-error');
        errors.push('Your signature\'s hex string is not the correct length');
    }

    // Show any errors we have
    if (errors.length > 0) {
        showErrors(errors);
        return;
    }

    // Construct JSON request
    var request = new Object();
    request.Address = address;
    request.Vote = answer;
    request.SignedVoteMessage = signature;

    // We want to get a list of all votes here
    apiCallXhr = new XMLHttpRequest();
    apiCallXhr.open("PUT", baseUrl + "referendum/" + id, true);
    apiCallXhr.setRequestHeader("Content-type", "application/json");
    apiCallXhr.onload = onApiSuccess;
    apiCallXhr.onerror = onApiError;
    apiCallXhr.onabort = onApiAbort;
    apiCallXhr.ontimeout = onApiTimeout;
    configurePostApiSuccess = function (json) {
        // Mark that we are done with this request
        apiCallXhr = null;

        // Update our record locally for this voter
        for (var i = 0; i < referendum.Electorate.length; i++) {
            var voter = referendum.Electorate[i];
            if (voter.Address !== address)
                continue;
            voter.Vote = answer;
            voter.SignedVoteMessage = signature;
            break;
        }

        // Update the UI
        recalculateVoters();
        recalculateResults();
        document.getElementById('castVoteSection').style.display = 'none';
        document.getElementById('voteSuccessful').style.display = null;

        // Reenable the UI
        document.getElementById("disabledOverlay").style.display = "none";
    }
    apiCallXhr.send(JSON.stringify(request));
}