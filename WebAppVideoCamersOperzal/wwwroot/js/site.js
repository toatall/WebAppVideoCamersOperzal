const containerErrorName = '#main-alert-danger'

function showError(text) {
    $(containerErrorName).children('span').html(text)
    $(containerErrorName).show()
}

function hideError() {
    $(containerErrorName).hide()
}

async function fetchText(url) {
    hideError()
    let responseResult = null
    const responseData = await fetch(url)
        .then(response => {
            responseResult = response
            return response.text()
        })
    if (!responseResult.ok) {
        showError(`${responseResult.status}: ${responseResult.statusText}`)
        console.error(responseResult)        
    }
    else {
        return responseData
    }
}

async function fetchPost(url, data) {
    hideError()
    let responseResult = null
    const responseData = await fetch(url, {
            method: 'POST',
            body: data
        })  
        .then(response => {
            responseResult = response
            return response.text()
        })
    if (!responseResult.ok) {
        showError(`${responseResult.status}: ${responseResult.statusText}`)
        console.error(responseResult)
    }
    else {
        return responseData
    }
}

$(document).on('click', '.btn-async', function () {
    console.log($(this).data('container'))
    fetchText($(this).attr('href'))
        .then(res => $($(this).data('container')).html(res))
    return false
})





