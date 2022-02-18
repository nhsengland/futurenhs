import * as React from 'react';
import { shallow } from 'enzyme';

import { DataGrid } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    text: {
        caption: 'Mock caption'
    },
    columnList: [],
    rowList: []
};

describe('Data Grid', () => {

	it('renders successfully', () => {

		const props = Object.assign({}, testProps);
		const wrapper = shallow(<DataGrid {...props} />);

		expect(wrapper.find('table').exists()).toBe(true);

	});

});
