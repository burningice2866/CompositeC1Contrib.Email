LogTableBinding.prototype = new Binding;
LogTableBinding.prototype.constructor = LogTableBinding;
LogTableBinding.superclass = Binding.prototype;

LogTableBinding.SELECTED = "reporttable version selected";
LogTableBinding.COMPARE = "reporttable compare selected";

LogTableBinding.CLASSNAME_COMPAREMODE = "comparemode";

LogTableBinding.selectedIndex = null;
LogTableBinding.scrollPoint = null;

/**
* True when version comparison is active.
* @type {boolean}
*/
LogTableBinding.isCompareMode = false;

/**
* @class
* @implements {IData}
*/
function LogTableBinding() {

    /**
	* @type {SystemLogger}
	*/
    this.logger = SystemLogger.getLogger("LogTableBinding");

    /*
	* Returnable.
	*/
    return this;
}

/**
* Identifies binding.
*/
LogTableBinding.prototype.toString = function () {

    return "[LogTableBinding]";
};

/**
* @overloads {Binding#onBindingAttach}
*/
LogTableBinding.prototype.onBindingAttach = function () {

    LogTableBinding.superclass.onBindingAttach.call(this);

    var row, rows = new List(this.bindingElement.rows);

    /*
	* Inject DOM events and index tokens. The latter 
	* is used to determine newest versus oldest entry. 
	* @see {LogTableBinding#isTokenNewer}
	*/
    while (rows.hasNext()) {
        row = rows.getNext();
        DOMEvents.addEventListener(row, DOMEvents.MOUSEENTER, this);
        DOMEvents.addEventListener(row, DOMEvents.MOUSELEAVE, this);
    }
};


/**
* @implements {IEventHandler}
* @param {MouseEvent} e
*/
LogTableBinding.prototype.handleEvent = function (e) {

    var target = e.currentTarget ? e.currentTarget : DOMEvents.getTarget(e);

    switch (e.type) {
        case DOMEvents.MOUSEENTER:
        case DOMEvents.MOUSEOVER:
            target.className = "hilite";
            break;
        case DOMEvents.MOUSELEAVE:
        case DOMEvents.MOUSEOUT:
            target.className = "";
            break;
    }
};