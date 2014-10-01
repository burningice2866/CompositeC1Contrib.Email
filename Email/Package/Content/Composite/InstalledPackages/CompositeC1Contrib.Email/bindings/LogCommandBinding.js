LogCommandBinding.prototype = new LabelBinding;
LogCommandBinding.prototype.constructor = LogCommandBinding;
LogCommandBinding.superclass = LabelBinding.prototype;

/**
* @class
* @implements {IData}
*/
function LogCommandBinding() {

	/**
	* @type {SystemLogger}
	*/
    this.logger = SystemLogger.getLogger("LogCommandBinding");

	/**.
	* @type {string}
	*/
	this.entityToken = null;

	/*
	* Returnable.
	*/
	return this;
}

/**
* Identifies binding.
*/
LogCommandBinding.prototype.toString = function () {

	return "[LogCommandBinding]";
};

/**
* Overloads {Binding#onBindingAttach}
*/
LogCommandBinding.prototype.onBindingAttach = function () {

	LogCommandBinding.superclass.onBindingAttach.call(this);

	this.link = this.getProperty("link");
};


/** 
* @overloads {LabelBinding#onBindingRegister}
*/
LogCommandBinding.prototype.onBindingRegister = function () {

	LogCommandBinding.superclass.onBindingRegister.call(this);

	this.addEventListener(DOMEvents.CLICK);

	this.attachClassName("entitytitle");
};

/**
* @implements {IEventListener}
* @overloads {Binding#handleEvent}
* @param {MouseEvent} e
*/
LogCommandBinding.prototype.handleEvent = function (e) {

	LogCommandBinding.superclass.handleEvent.call(this, e);

	switch (e.type) {
		case DOMEvents.CLICK:
			if (this.link) {
			    window.location = this.link;
			}

			break;
	}
}