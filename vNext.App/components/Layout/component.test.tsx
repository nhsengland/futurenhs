import { render, screen } from '@testing-library/react';
import { shallow } from 'enzyme';

import { Layout } from './index';

import { Props } from './interfaces';

const testProps: Props = {
    children: undefined
};

describe('Layout', () => {
    
    it('renders page content', () => {

        const props = Object.assign({}, testProps);

        render(<Layout {...props} ><p>Mock page content</p></Layout>);

        expect(screen.getByText('Mock page content'));

    });

    it('Includes a custom class name', () => {

        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class'
        });
        const wrapper = shallow(<Layout {...testProps} />);
        const wrapperCustomClass = shallow(<Layout {...propsCustomClass} />);

        expect(wrapper.find('.mock-class').exists()).toBe(false);
        expect(wrapperCustomClass.find('.mock-class').exists()).toBe(true);

    });
    
});
