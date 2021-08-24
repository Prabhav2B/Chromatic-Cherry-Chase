var OpenWindowPlugin = {
    OpenWindow: function(link)
    {
    	var url = Pointer_stringify(link);
        window.open(url,'_blank').focus();
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);