import { render, screen } from '@testing-library/react';
import { shallow } from 'enzyme';

import { GroupLayout } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    id: 'index'
};

describe('Group Layout', () => {

    it('Includes a custom class name', () => {

        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class'
        });
        const wrapper = shallow(<GroupLayout {...testProps} />);
        const wrapperCustomClass = shallow(<GroupLayout {...propsCustomClass} />);

        expect(wrapper.find('.mock-class').exists()).toBe(false);
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true);

    });
    
});
