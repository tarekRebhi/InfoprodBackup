var downloadPDF = function () {
    DocRaptor.createAndDownloadDoc("YOUR_API_KEY_HERE", {
        test: true, // test documents are free, but watermarked
        type: "pdf",

        document_content: (document.querySelector('#toPDF')).innerHTML, // use this page's HTML
        strict: 'none',
        javascript: 'true',
        // document_content: "<h1>Hello world!</h1>",               // or supply HTML directly
        // document_url: "http://example.com/your-page",            // or use a URL
        // javascript: true,                                        // enable JavaScript processing
        //prince_options: {
        // media: "print",                                         // use screen styles instead of print styles
        // }
    });
}