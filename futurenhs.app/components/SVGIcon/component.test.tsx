import { shallow } from 'enzyme';

import { SVGIcon } from './index';
import { getEnvVar } from '@helpers/util/env';

import { Props } from './interfaces';

const testProps: Props = {
    name: 'mock-icon'
};

describe('SVG Icon component', () => {

    it('renders successfully', () => {

        const props = Object.assign({}, testProps);
        const wrapper = shallow(<SVGIcon {...props} />);

        expect(wrapper.find('svg.c-svg-icon').exists()).toBe(true);

    });

    it('Requests the expected icon id', () => {

        const propsNoIconUrl = Object.assign({}, testProps, {
            url: null
        });
        const wrapper = shallow(<SVGIcon {...testProps} />);
        const wrapperNoIconUrl = shallow(<SVGIcon {...propsNoIconUrl} />);
        const url: string = process.env.NEXT_PUBLIC_ASSET_PREFIX ? process.env.NEXT_PUBLIC_ASSET_PREFIX + '/icons/icons.svg' : '/icons/icons.svg';

        expect(wrapper.find('svg.c-svg-icon use').prop('xlinkHref')).toEqual(`${url}#mock-icon`);
        expect(wrapperNoIconUrl.find('svg.c-svg-icon use').prop('xlinkHref')).toEqual('#mock-icon');

    });

    it('Includes a custom class name', () => {

        const propsCustomClass = Object.assign({}, testProps, {
            className: 'mock-class'
        });
        const wrapper = shallow(<SVGIcon {...testProps} />);
        const wrapperCustomClass = shallow(<SVGIcon {...propsCustomClass} />);

        expect(wrapper.find('svg.mock-class').exists()).toBe(false);
        expect(wrapperCustomClass.find('svg.mock-class').exists()).toBe(true);

    });

});