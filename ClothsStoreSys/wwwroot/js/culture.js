window.blazorSetCulture = function (culture) {
    // set cookie for culture
    document.cookie = '.AspNetCore.Culture=c=' + culture + '|uic=' + culture + ';path=/';

    // reload page to apply culture change
    location.reload();
};
